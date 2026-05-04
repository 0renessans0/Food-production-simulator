const express = require('express');
const cors = require('cors');
const dotenv = require('dotenv');
const pool = require('./src/config/database');

dotenv.config();

const app = express();
const PORT = process.env.PORT || 3000;

app.use(cors());
app.use(express.json());

// ============ API ЭНДПОИНТЫ ============
app.use((req, res, next) => {
    res.setHeader('Content-Type', 'application/json; charset=utf-8');
    next();
});
// Получение технического регламента
app.get('/api/regulations', async (req, res) => {
    try {
        const result = await pool.query('SELECT * FROM Regulations WHERE id = 1');
        res.json(result.rows[0]);
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Ошибка загрузки регламента' });
    }
});

// Получение правил игры
app.get('/api/rules', async (req, res) => {
    try {
        const result = await pool.query('SELECT * FROM GameRules WHERE id = 1');
        res.json(result.rows[0]);
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: 'Ошибка загрузки правил' });
    }
});

// Сохранение результатов
app.post('/api/results', async (req, res) => {
    const { session_id, grade, error_report, regulation_id, rules_id } = req.body;
    
    try {
        const result = await pool.query(
            `INSERT INTO GameResults (session_id, regulation_id, rules_id, grade, error_report) 
             VALUES ($1, $2, $3, $4, $5) RETURNING id`,
            [session_id, regulation_id || 1, rules_id || 1, grade, error_report]
        );
        console.log('✅ Результаты сохранены в БД, id:', result.rows[0].id);
        res.json({ status: 'ok', id: result.rows[0].id, message: 'Результаты сохранены' });
    } catch (err) {
        console.error('❌ Ошибка сохранения:', err);
        res.status(500).json({ error: 'Ошибка сохранения результатов' });
    }
});

app.get('/api/results', async (req, res) => {
    try {
        const result = await pool.query('SELECT * FROM GameResults ORDER BY id DESC');
        console.log('📊 Получен запрос на /api/results, найдено записей:', result.rows.length);
        res.json(result.rows);
    } catch (err) {
        console.error('❌ Ошибка загрузки результатов:', err);
        res.status(500).json({ error: err.message });
    }
});

// Проверка работы сервера
app.get('/', (req, res) => {
    res.json({ message: 'Сервер симулятора пивоварения работает!' });
});

app.listen(PORT, () => {
    console.log(`🚀 Сервер запущен на http://localhost:${PORT}`);
    console.log(`📋 Доступные эндпоинты:`);
    console.log(`   GET  /api/regulations`);
    console.log(`   GET  /api/rules`);
    console.log(`   POST /api/results`);
    console.log(`   GET  /api/results`);
});