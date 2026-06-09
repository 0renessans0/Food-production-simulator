const { Pool } = require('pg');
require('dotenv').config();

const pool = new Pool({
    host: process.env.DB_HOST,
    port: process.env.DB_PORT,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME,
});

pool.connect((err, client, release) => {
    if (err) {
        console.error('не подключено к бд:', err.message);
    } else {
        console.log('бд подключена');
        release();
    }
});

pool.on('error', (err) => {
    console.error('ошибка пула БД:', err.message);
});

module.exports = pool;
