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

// ----------------- SERVER BAŞLAT -----------------
app.listen(3000, () => {
  console.log("Backend çalışıyor: http://localhost:3000 ✔️");
});
