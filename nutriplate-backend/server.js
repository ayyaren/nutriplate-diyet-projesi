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
        req.user = decoded; // { userId, role }
        next();
    } catch (err) {
        return res.status(401).json({ error: "Token geçersiz" });
    }
};

// ----------------- SADECE DİYETİSYEN MIDDLEWARE -----------------
// role_id: 1 = User, 2 = Dietitian, 3 = Admin
const dietitianOnly = (req, res, next) => {
    if (req.user.role !== 2) {
        return res.status(403).json({ error: "DietitianOnly" });
    }
    next();
};

// ======================================================================
// AUTH
// ======================================================================

// ----------------- REGISTER (NORMAL KULLANICI) -----------------
app.post("/api/auth/register", (req, res) => {
    console.log(">>> /api/auth/register çağrıldı. Body:", req.body);

    const { full_name, email, password } = req.body;

    if (!full_name || !email || !password) {
        console.log("Eksik alanlar:", { full_name, email, password });
        return res
            .status(400)
            .json({ error: "full_name, email ve password zorunludur" });
    }

    const checkQuery = "SELECT id FROM users WHERE email = ?";
    db.query(checkQuery, [email], (err, rows) => {
        if (err) {
            console.error("Email kontrol hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        if (rows.length > 0) {
            console.log("Bu email zaten kayıtlı:", email);
            return res.status(409).json({ error: "EmailAlreadyExists" });
        }

        // Normal kullanıcı için role_id = 1 (User)
        const insertQuery = `
      INSERT INTO users (full_name, email, password_hash, role_id)
      VALUES (?, ?, ?, 1)
    `;

        db.query(insertQuery, [full_name, email, password], (err2, result) => {
            if (err2) {
                console.error("Kayıt INSERT hatası:", err2.sqlMessage || err2);
                return res.status(500).json({ error: "db_insert_error" });
            }

            console.log("✅ Kullanıcı kaydedildi, id:", result.insertId);

            res.status(201).json({
                message: "Kullanıcı başarıyla oluşturuldu",
                userId: result.insertId,
            });
        });
    });
});

// ----------------- REGISTER (DİYETİSYEN) -----------------
app.post("/api/auth/dietitian-register", (req, res) => {
    console.log(">>> /api/auth/dietitian-register çağrıldı. Body:", req.body);

    const { full_name, email, password } = req.body;

    if (!full_name || !email || !password) {
        console.log("Eksik alanlar (diyetisyen):", { full_name, email, password });
        return res
            .status(400)
            .json({ error: "full_name, email ve password zorunludur" });
    }

    const checkQuery = "SELECT id FROM users WHERE email = ?";
    db.query(checkQuery, [email], (err, rows) => {
        if (err) {
            console.error("Diyetisyen email kontrol hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        if (rows.length > 0) {
            console.log("Bu email zaten kayıtlı (diyetisyen):", email);
            return res.status(409).json({ error: "EmailAlreadyExists" });
        }

        // Dietitian için role_id = 2
        const insertQuery = `
      INSERT INTO users (full_name, email, password_hash, role_id)
      VALUES (?, ?, ?, 2)
    `;

        db.query(insertQuery, [full_name, email, password], (err2, result) => {
            if (err2) {
                console.error("Diyetisyen INSERT hatası:", err2.sqlMessage || err2);
                return res.status(500).json({ error: "db_insert_error" });
            }

            console.log("✅ Diyetisyen kaydedildi, id:", result.insertId);

            res.status(201).json({
                message: "Diyetisyen başarıyla oluşturuldu",
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
        if (err) {
            console.error("Login SQL hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        if (results.length === 0) {
            return res.status(401).json({ error: "InvalidCredentials" });
        }

        const user = results[0];

        // password_hash kolonunu kullanıyoruz
        const isMatch = user.password_hash === password;

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
        if (err) {
            console.error("/api/auth/me sorgu hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        if (rows.length === 0) {
            return res.status(404).json({ error: "UserNotFound" });
        }

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

// ======================================================================
// PROFİL (KULLANICI)
// ======================================================================

// GET /api/profile – aktif kullanıcının profilini getir
app.get("/api/profile", authMiddleware, (req, res) => {
    const userId = req.user.userId;

    const sql = `
        SELECT 
            full_name            AS name,
            email,
            birth_date           AS birthDate,
            gender,
            activity_level       AS activityLevel,
            height_cm            AS heightCm,
            weight_kg            AS weightKg,
            daily_calorie_target AS dailyCalorieTarget
        FROM users
        WHERE id = ?
    `;

    db.query(sql, [userId], (err, rows) => {
        if (err) {
            console.error("Profile GET error:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        if (rows.length === 0) {
            return res.status(404).json({ error: "UserNotFound" });
        }

        res.json(rows[0]);
    });
});

// PUT /api/profile – aktif kullanıcının profilini güncelle
app.put("/api/profile", authMiddleware, (req, res) => {
    const userId = req.user.userId;

    const {
        name,
        birthDate,
        gender,
        activityLevel,
        heightCm,
        weightKg,
        dailyCalorieTarget,
    } = req.body;

    const sql = `
        UPDATE users
        SET
            full_name            = ?,
            birth_date           = ?,
            gender               = ?,
            activity_level       = ?,
            height_cm            = ?,
            weight_kg            = ?,
            daily_calorie_target = ?
        WHERE id = ?
    `;

    db.query(
        sql,
        [
            name,
            birthDate || null,
            gender || null,
            activityLevel || null,
            heightCm || null,
            weightKg || null,
            dailyCalorieTarget || null,
            userId,
        ],
        (err, result) => {
            if (err) {
                console.error("Profile PUT error:", err.sqlMessage || err);
                return res.status(500).json({ error: "db_error" });
            }

            return res.json({ success: true });
        }
    );
});

// ======================================================================
// DİYETİSYEN ENDPOINT'LERİ
// ======================================================================

// Danışan listesi (role_id = 1 olan tüm kullanıcılar)
app.get("/api/dietitian/clients", authMiddleware, dietitianOnly, (req, res) => {
    const sql = `
    SELECT id, full_name, email, created_at
    FROM users
    WHERE role_id = 1
    ORDER BY full_name
  `;

    db.query(sql, (err, rows) => {
        if (err) {
            console.error("Diyetisyen client listesi hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        res.json(rows);
    });
});

// Belirli bir danışanın günlük kalori özeti
app.get(
    "/api/dietitian/client/:id/daily-summary",
    authMiddleware,
    dietitianOnly,
    (req, res) => {
        const clientId = Number(req.params.id);

        if (!clientId) {
            return res.status(400).json({ error: "Geçersiz client id" });
        }

        const sql = `
      SELECT 
        DATE(meal_datetime) AS summary_date,
        SUM(total_calories) AS total_calories
      FROM meals
      WHERE user_id = ?
      GROUP BY DATE(meal_datetime)
      ORDER BY summary_date DESC
    `;

        db.query(sql, [clientId], (err, rows) => {
            if (err) {
                console.error(
                    "Diyetisyen client daily-summary hatası:",
                    err.sqlMessage || err
                );
                return res.status(500).json({ error: "db_error" });
            }

            res.json(rows);
        });
    }
);

// ======================================================================
// MEALS
// ======================================================================

// ---------- HOME SAYFASI İÇİN GENEL GET /api/meals (AUTH YOK) ----------
app.get("/api/meals", (req, res) => {
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
    ORDER BY meal_datetime DESC
    LIMIT 50
  `;

    db.query(sql, (err, rows) => {
        if (err) {
            console.error("Genel meals listesi hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        res.json(rows);
    });
});

// ----------------- GET /api/meals/day?date=YYYY-MM-DD -----------------
// Aktif kullanıcının seçili gündeki öğünleri
app.get("/api/meals/day", authMiddleware, (req, res) => {
    const userId = req.user.userId;
    const { date } = req.query; // "2025-12-04" gibi

    if (!date) {
        return res
            .status(400)
            .json({ error: "date parametresi zorunludur (YYYY-MM-DD)" });
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
      AND DATE(meal_datetime) = ?
    ORDER BY meal_datetime
  `;

    db.query(sql, [userId, date], (err, rows) => {
        if (err) {
            console.error("Günlük meal listesi hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        res.json(rows);
    });
});

// ----------------- GET /api/meals/day-summary?date=YYYY-MM-DD ---------
// Aktif kullanıcının seçili gün için toplam kalori özeti
app.get("/api/meals/day-summary", authMiddleware, (req, res) => {
    const userId = req.user.userId;
    const { date } = req.query;

    if (!date) {
        return res
            .status(400)
            .json({ error: "date parametresi zorunludur (YYYY-MM-DD)" });
    }

    const sql = `
    SELECT 
      IFNULL(SUM(total_calories), 0) AS total_calories
    FROM meals
    WHERE user_id = ?
      AND DATE(meal_datetime) = ?
  `;

    db.query(sql, [userId, date], (err, rows) => {
        if (err) {
            console.error("Günlük özet hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        const row = rows[0] || { total_calories: 0 };
        res.json({
            date,
            totalCalories: row.total_calories,
        });
    });
});

// ----------------- POST /api/meals (manuel ekleme) -----------------
app.post("/api/meals", authMiddleware, (req, res) => {
    console.log("POST /api/meals endpoint çalıştı ✔️");
    console.log("Body:", req.body);

    const userId = req.user.userId;
    const { meal_datetime, meal_type, total_calories, note, items } = req.body;

    if (!meal_datetime || !meal_type) {
        return res
            .status(400)
            .json({ error: "meal_datetime ve meal_type zorunludur" });
    }

    const insertMealSql = `
    INSERT INTO meals (user_id, meal_datetime, meal_type, total_calories, note)
    VALUES (?, ?, ?, ?, ?)
  `;

    db.query(
        insertMealSql,
        [userId, meal_datetime, meal_type, total_calories ?? 0, note || null],
        (err, result) => {
            if (err) {
                console.error("Meal ekleme hatası:", err.sqlMessage || err);
                return res.status(500).json({ error: "db_error_meal_insert" });
            }

            const mealId = result.insertId;
            console.log("✅ Meal eklendi, id:", mealId);

            // items yoksa direkt dön
            if (!Array.isArray(items) || items.length === 0) {
                return res.status(201).json({
                    message: "Meal başarıyla eklendi",
                    mealId,
                });
            }

            // --- YEMEK KALEMLERİNİ (items) KAYDET ---
            items.forEach((it) => {
                if (!it || !it.foodName) return;

                const foodName = it.foodName.trim();
                const gram = Number(it.gram) || 0;
                const kcal = Number(it.kcal) || 0;

                const kcalPer100 =
                    gram > 0 && kcal > 0 ? Math.round((kcal * 100) / gram) : null;

                // 1) foods tablosunda var mı?
                const selectFoodSql = "SELECT id FROM foods WHERE name = ?";
                db.query(selectFoodSql, [foodName], (errSel, foodRows) => {
                    if (errSel) {
                        console.error("Food select hatası:", errSel.sqlMessage || errSel);
                        return;
                    }

                    const insertMealFood = (foodId) => {
                        const mfSql = `
                            INSERT INTO mealfoods (meal_id, food_id, quantity, created_at)
                            VALUES (?, ?, ?, NOW())
                        `;
                        db.query(mfSql, [mealId, foodId, gram], (errMf) => {
                            if (errMf) {
                                console.error(
                                    "mealfoods insert hatası:",
                                    errMf.sqlMessage || errMf
                                );
                            }
                        });
                    };

                    if (foodRows.length > 0) {
                        // Zaten kayıtlı
                        insertMealFood(foodRows[0].id);
                    } else {
                        // 2) Yeni food kaydı (foods tablosuna göre)
                        const insertFoodSql = `
                            INSERT INTO foods (name, kcal_per_100g)
                            VALUES (?, ?)
                        `;
                        db.query(
                            insertFoodSql,
                            [foodName, kcalPer100],
                            (errIns, foodRes) => {
                                if (errIns) {
                                    console.error(
                                        "foods insert hatası:",
                                        errIns.sqlMessage || errIns
                                    );
                                    return;
                                }
                                insertMealFood(foodRes.insertId);
                            }
                        );
                    }
                });
            });

            return res.status(201).json({
                message: "Meal + items başarıyla eklendi",
                mealId,
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
        ml_result,
    } = req.body;

    if (!meal_datetime || !meal_type || !image_url) {
        return res.status(400).json({
            error: "meal_datetime, meal_type ve image_url zorunludur",
        });
    }

    const insertMealSql = `
    INSERT INTO meals (user_id, meal_datetime, meal_type, total_calories, note)
    VALUES (?, ?, ?, ?, ?)
  `;

    db.query(
        insertMealSql,
        [userId, meal_datetime, meal_type, total_calories || 0, note || null],
        (err, mealResult) => {
            if (err) {
                console.error("from-photo meal ekleme hatası:", err.sqlMessage || err);
                return res.status(500).json({ error: "db_error" });
            }

            const mealId = mealResult.insertId;

            const insertImageSql = `
        INSERT INTO mealimages (meal_id, image_url, ml_result)
        VALUES (?, ?, ?)
      `;

            db.query(
                insertImageSql,
                [mealId, image_url, ml_result ? JSON.stringify(ml_result) : null],
                (err2) => {
                    if (err2) {
                        console.error("mealimages kaydı hatası:", err2.sqlMessage || err2);
                        return res.status(500).json({ error: "db_error" });
                    }

                    res.status(201).json({
                        message: "Fotoğraftan meal kaydedildi ✔️",
                        mealId: mealId,
                    });
                }
            );
        }
    );
});

// ----------------- GET /api/meals/user/:id -----------------
app.get("/api/meals/user/:id", authMiddleware, (req, res) => {
    const requestedUserId = parseInt(req.params.id, 10);

    if (Number.isNaN(requestedUserId)) {
        return res.status(400).json({ error: "Geçersiz kullanıcı id" });
    }

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
            console.error("Kullanıcı öğünleri çekme hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        res.json(rows);
    });
});

// ----------------- GET /api/meals/:mealId -----------------
app.get("/api/meals/:mealId", authMiddleware, (req, res) => {
    const mealId = Number(req.params.mealId);

    if (!mealId) {
        return res.status(400).json({ error: "Geçersiz mealId" });
    }

    const sqlMeal = `
    SELECT id, user_id, meal_datetime, meal_type, total_calories, note, created_at
    FROM meals
    WHERE id = ?
  `;

    db.query(sqlMeal, [mealId], (err, rows) => {
        if (err) {
            console.error("Meal detay hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        if (rows.length === 0) {
            return res.status(404).json({ error: "Meal bulunamadı" });
        }

        const meal = rows[0];

        if (meal.user_id !== req.user.userId) {
            return res.status(403).json({ error: "Bu meal'e erişemezsin" });
        }

        // Yemek satırlarını çek – kcal_per_100g ile kcal hesapla
        const sqlItems = `
        SELECT 
            f.name AS foodName,
            mf.quantity AS gram,
            CASE 
              WHEN f.kcal_per_100g IS NULL THEN NULL
              ELSE ROUND(f.kcal_per_100g * mf.quantity / 100)
            END AS kcal
        FROM mealfoods mf
        JOIN foods f ON f.id = mf.food_id
        WHERE mf.meal_id = ?
        ORDER BY mf.id
      `;

        db.query(sqlItems, [mealId], (err2, itemRows) => {
            if (err2) {
                console.error("Meal item sorgu hatası:", err2.sqlMessage || err2);
                return res.status(500).json({ error: "db_error_items" });
            }

            res.json({
                id: meal.id,
                user_id: meal.user_id,
                meal_datetime: meal.meal_datetime,
                meal_type: meal.meal_type,
                total_calories: meal.total_calories,
                note: meal.note,
                created_at: meal.created_at,
                items: itemRows || [],
            });
        });
    });
});

// ----------------- DELETE /api/meals/:mealId -----------------
app.delete("/api/meals/:mealId", authMiddleware, (req, res) => {
    const mealId = Number(req.params.mealId);

    if (!mealId) {
        return res.status(400).json({ error: "Geçersiz mealId" });
    }

    const checkSql = "SELECT user_id FROM meals WHERE id = ?";

    db.query(checkSql, [mealId], (err, rows) => {
        if (err) {
            console.error("Meal kontrol hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        if (rows.length === 0) {
            return res.status(404).json({ error: "Meal bulunamadı" });
        }

        const ownerId = rows[0].user_id;

        if (ownerId !== req.user.userId) {
            return res.status(403).json({ error: "Bu meal'i silemezsin" });
        }

        const deleteSql = "DELETE FROM meals WHERE id = ?";

        db.query(deleteSql, [mealId], (err2, result) => {
            if (err2) {
                console.error("Meal silme hatası:", err2.sqlMessage || err2);
                return res.status(500).json({ error: "db_error" });
            }

            res.json({
                message: "Meal silindi",
                affectedRows: result.affectedRows,
            });
        });
    });
});

// ======================================================================
// RECOMMENDATIONS & SUMMARY
// ======================================================================

// ----------------- GET /api/recommendations/:mealId -----------------
app.get("/api/recommendations/:mealId", authMiddleware, (req, res) => {
    const mealId = Number(req.params.mealId);

    if (!mealId) {
        return res.status(400).json({ error: "Geçersiz mealId" });
    }

    const sqlMeal = `SELECT user_id FROM meals WHERE id = ?`;

    db.query(sqlMeal, [mealId], (err, rows) => {
        if (err) {
            console.error("Meal kontrol hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        if (rows.length === 0) {
            return res.status(404).json({ error: "Meal bulunamadı" });
        }

        const ownerId = rows[0].user_id;

        if (ownerId !== req.user.userId) {
            return res
                .status(403)
                .json({ error: "Bu meal için önerilere erişemezsin" });
        }

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
                console.error("Recommendation sorgu hatası:", err2.sqlMessage || err2);
                return res.status(500).json({ error: "db_error" });
            }

            res.json({
                mealId,
                user_id: ownerId,
                recommendations: recRows,
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

    if (targetUserId !== req.user.userId) {
        return res
            .status(403)
            .json({ error: "Bu kullanıcının özetini göremezsin" });
    }

    const sql = `
    SELECT *
    FROM vw_userdailysummary
    WHERE user_id = ?
    ORDER BY summary_date DESC
  `;

    db.query(sql, [targetUserId], (err, rows) => {
        if (err) {
            console.error("Daily summary hatası:", err.sqlMessage || err);
            return res.status(500).json({ error: "db_error" });
        }

        res.json(rows);
    });
});

// ----------------- SERVER BAŞLAT -----------------
app.listen(3000, () => {
    console.log("Backend çalışıyor: http://localhost:3000 ✔️");
});
