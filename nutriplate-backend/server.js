import express from "express";
import cors from "cors";
import jwt from "jsonwebtoken";
import mysql from "mysql2";

const app = express();
app.use(express.json());
app.use(cors());

// ----------------- MySQL BAĞLANTISI -----------------
const db = mysql.createConnection({
  host: "localhost",
  user: "root",
  password: "Ceren237363!",
  database: "nutriplate",
});

db.connect((err) => {
  if (err) {
    console.error("MySQL bağlantı hatası:", err);
    process.exit(1);
  }
  console.log("MySQL bağlantısı başarılı ✔️");
});

// ----------------- JWT DOĞRULAMA -----------------
const authMiddleware = (req, res, next) => {
  const authHeader = req.headers.authorization;
  if (!authHeader) {
    return res.status(401).json({ error: "Token bulunamadı" });
  }

  const token = authHeader.split(" ")[1];

  try {
    const decoded = jwt.verify(token, "verySecretKey123");
    req.user = decoded;
    next();
  } catch (err) {
    return res.status(401).json({ error: "Token geçersiz" });
  }
};

// ----------------- REGISTER -----------------
app.post("/api/auth/register", (req, res) => {
  const { full_name, email, password } = req.body;

  const checkQuery = "SELECT id FROM users WHERE email = ?";
  db.query(checkQuery, [email], (err, rows) => {
    if (rows.length > 0) {
      return res.status(409).json({ error: "Email zaten kayıtlı" });
    }

    const insertQuery = `
      INSERT INTO users (full_name, email, password, role_id)
      VALUES (?, ?, ?, 1)
    `;

    db.query(insertQuery, [full_name, email, password], (err2, result) => {
      res.status(201).json({
        message: "Kullanıcı başarıyla oluşturuldu",
        userId: result.insertId,
      });
    });
  });
});

// ----------------- LOGIN -----------------
app.post("/api/auth/login", (req, res) => {
  const { email, password } = req.body;

  const sql = "SELECT * FROM users WHERE email = ?";
  db.query(sql, [email], (err, results) => {
    if (results.length === 0) {
      return res.status(401).json({ error: "InvalidCredentials" });
    }

    const user = results[0];
    const isMatch = user.password === password;

    if (!isMatch) {
      return res.status(401).json({ error: "InvalidCredentials" });
    }

    const token = jwt.sign(
      { userId: user.id, role: user.role_id },
      "verySecretKey123",
      { expiresIn: "2h" }
    );

    res.json({
      token,
      userId: user.id,
      name: user.full_name,
      email: user.email,
      role: user.role_id,
    });
  });
});

// ----------------- GET /api/auth/me -----------------
app.get("/api/auth/me", authMiddleware, (req, res) => {
  const userId = req.user.userId;

  const sql = "SELECT id, full_name, email, role_id FROM users WHERE id = ?";
  db.query(sql, [userId], (err, rows) => {
    const user = rows[0];
    res.json({
      id: user.id,
      name: user.full_name,
      email: user.email,
      role: user.role_id,
      message: "Token doğrulandı ✔️",
    });
  });
});

// ----------------- POST /api/meals (manuel ekleme) -----------------
app.post("/api/meals", authMiddleware, (req, res) => {
  console.log("POST /api/meals endpoint çalıştı ✔️");

  const userId = req.user.userId;
  const { meal_datetime, meal_type, total_calories, note } = req.body;

  const sql = `
    INSERT INTO meals (user_id, meal_datetime, meal_type, total_calories, note)
    VALUES (?, ?, ?, ?, ?)
  `;

  db.query(
    sql,
    [userId, meal_datetime, meal_type, total_calories, note || null],
    (err, result) => {
      if (err) {
        console.error("Meal ekleme hatası:", err);
        return res.status(500).json({ error: "db_error" });
      }

      res.status(201).json({
        message: "Meal başarıyla eklendi",
        mealId: result.insertId,
      });
    }
  );
});
// ----------------- POST /api/meals/from-photo -----------------
app.post("/api/meals/from-photo", authMiddleware, (req, res) => {
  const userId = req.user.userId;

  const {
    meal_datetime,
    meal_type,
    total_calories,
    note,
    image_url,
    ml_result
  } = req.body;

  // Zorunlu alanlar
  if (!meal_datetime || !meal_type || !image_url) {
    return res.status(400).json({
      error: "meal_datetime, meal_type ve image_url zorunludur"
    });
  }

  // 1) Önce meals tablosuna kayıt
  const insertMealSql = `
    INSERT INTO meals (user_id, meal_datetime, meal_type, total_calories, note)
    VALUES (?, ?, ?, ?, ?)
  `;

  db.query(
    insertMealSql,
    [userId, meal_datetime, meal_type, total_calories || 0, note || null],
    (err, mealResult) => {
      if (err) {
        console.error("from-photo meal ekleme hatası:", err);
        return res.status(500).json({ error: "db_error" });
      }

      const mealId = mealResult.insertId;

      // 2) mealimages tablosuna fotoğraf + ML sonucu kaydı
      const insertImageSql = `
        INSERT INTO mealimages (meal_id, image_url, ml_result)
        VALUES (?, ?, ?)
      `;

      db.query(
        insertImageSql,
        [mealId, image_url, ml_result ? JSON.stringify(ml_result) : null],
        (err2) => {
          if (err2) {
            console.error("mealimages kaydı hatası:", err2);
            return res.status(500).json({ error: "db_error" });
          }

          // 3) Başarılı cevap
          res.status(201).json({
            message: "Fotoğraftan meal kaydedildi ✔️",
            mealId: mealId
          });
        }
      );
    }
  );
});

// ----------------- GET /api/meals/user/:id -----------------
app.get("/api/meals/user/:id", authMiddleware, (req, res) => {
  const requestedUserId = parseInt(req.params.id, 10);

  // id sayı mı kontrolü
  if (Number.isNaN(requestedUserId)) {
    return res.status(400).json({ error: "Geçersiz kullanıcı id" });
  }

  // Kullanıcı sadece kendi id'sini görebilsin
  if (requestedUserId !== req.user.userId) {
    return res
      .status(403)
      .json({ error: "Bu kullanıcının öğünlerine erişemezsin" });
  }

  const sql = `
    SELECT
      id,
      user_id,
      meal_datetime,
      meal_type,
      total_calories,
      note,
      created_at
    FROM meals
    WHERE user_id = ?
    ORDER BY meal_datetime DESC
  `;

  db.query(sql, [requestedUserId], (err, rows) => {
    if (err) {
      console.error("Kullanıcı öğünleri çekme hatası:", err);
      return res.status(500).json({ error: "db_error" });
    }

    // Hiç öğün yoksa da [] döner, bu normal
    res.json(rows);
  });
});
// ----------------- GET /api/meals/:mealId -----------------
app.get("/api/meals/:mealId", authMiddleware, (req, res) => {
  const mealId = Number(req.params.mealId);

  if (!mealId) {
    return res.status(400).json({ error: "Geçersiz mealId" });
  }

  const sql = `
    SELECT id, user_id, meal_datetime, meal_type, total_calories, note, created_at
    FROM meals
    WHERE id = ?
  `;

  db.query(sql, [mealId], (err, rows) => {
    if (err) {
      console.error("Meal detay hatası:", err);
      return res.status(500).json({ error: "db_error" });
    }

    if (rows.length === 0) {
      return res.status(404).json({ error: "Meal bulunamadı" });
    }

    const meal = rows[0];

    // Kullanıcının kendi meal'i mi?
    if (meal.user_id !== req.user.userId) {
      return res.status(403).json({ error: "Bu meal'e erişemezsin" });
    }

    res.json(meal);
  });
});

// ----------------- DELETE /api/meals/:mealId -----------------
app.delete("/api/meals/:mealId", authMiddleware, (req, res) => {
  const mealId = Number(req.params.mealId);

  if (!mealId) {
    return res.status(400).json({ error: "Geçersiz mealId" });
  }

  // Önce meal kullanıcıya mı ait kontrol et
  const checkSql = "SELECT user_id FROM meals WHERE id = ?";

  db.query(checkSql, [mealId], (err, rows) => {
    if (err) {
      console.error("Meal kontrol hatası:", err);
      return res.status(500).json({ error: "db_error" });
    }

    if (rows.length === 0) {
      return res.status(404).json({ error: "Meal bulunamadı" });
    }

    const ownerId = rows[0].user_id;

    if (ownerId !== req.user.userId) {
      return res.status(403).json({ error: "Bu meal'i silemezsin" });
    }

    // Meal gerçekten kullanıcıya ait → şimdi silelim
    const deleteSql = "DELETE FROM meals WHERE id = ?";

    db.query(deleteSql, [mealId], (err2, result) => {
      if (err2) {
        console.error("Meal silme hatası:", err2);
        return res.status(500).json({ error: "db_error" });
      }

      res.json({
        message: "Meal silindi",
        affectedRows: result.affectedRows
      });
    });
  });
});

// ----------------- GET /api/recommendations/:mealId -----------------
app.get("/api/recommendations/:mealId", authMiddleware, (req, res) => {
  const mealId = Number(req.params.mealId);

  if (!mealId) {
    return res.status(400).json({ error: "Geçersiz mealId" });
  }

  // 1) Meal kime ait bulunur
  const sqlMeal = `SELECT user_id FROM meals WHERE id = ?`;

  db.query(sqlMeal, [mealId], (err, rows) => {
    if (err) {
      console.error("Meal kontrol hatası:", err);
      return res.status(500).json({ error: "db_error" });
    }

    if (rows.length === 0) {
      return res.status(404).json({ error: "Meal bulunamadı" });
    }

    const ownerId = rows[0].user_id;

    // Kullanıcı sadece kendi meal önerisini görebilir
    if (ownerId !== req.user.userId) {
      return res.status(403).json({ error: "Bu meal için önerilere erişemezsin" });
    }

    // 2) User'ın tüm recommendations kayıtlarını getir
    const sqlRec = `
      SELECT r.id, r.user_id, r.dietitian_id, r.recommendation_text, r.created_at,
             u.full_name AS dietitian_name
      FROM recommendations r
      LEFT JOIN users u ON u.id = r.dietitian_id
      WHERE r.user_id = ?
      ORDER BY r.created_at DESC
    `;

    db.query(sqlRec, [ownerId], (err2, recRows) => {
      if (err2) {
        console.error("Recommendation sorgu hatası:", err2);
        return res.status(500).json({ error: "db_error" });
      }

      res.json({
        mealId,
        user_id: ownerId,
        recommendations: recRows
      });
    });
  });
});

// ----------------- GET /api/users/:id/daily-summary -----------------
app.get("/api/users/:id/daily-summary", authMiddleware, (req, res) => {
  const targetUserId = Number(req.params.id);

  if (!targetUserId) {
    return res.status(400).json({ error: "Geçersiz kullanıcı id" });
  }

  // Kullanıcı sadece kendi özetini görebilir
  if (targetUserId !== req.user.userId) {
    return res.status(403).json({ error: "Bu kullanıcının özetini göremezsin" });
  }

  const sql = `
    SELECT *
    FROM vw_userdailysummary
    WHERE user_id = ?
    ORDER BY summary_date DESC
  `;

  db.query(sql, [targetUserId], (err, rows) => {
    if (err) {
      console.error("Daily summary hatası:", err);
      return res.status(500).json({ error: "db_error" });
    }

    res.json(rows);
  });
});


// ----------------- SERVER BAŞLAT -----------------
app.listen(3000, () => {
  console.log("Backend çalışıyor: http://localhost:3000 ✔️");
});
