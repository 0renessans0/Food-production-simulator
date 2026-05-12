const pool = require('../config/database');

async function getRegulations() {
    const result = await pool.query('SELECT * FROM Regulations WHERE id = 1');
    return result.rows[0];
}

async function getGameRules() {
    const result = await pool.query('SELECT * FROM GameRules WHERE id = 1');
    return result.rows[0];
}

async function saveResult(session_id, regulation_id, rules_id, grade, error_report) {
    const result = await pool.query(
        `INSERT INTO GameResults (session_id, regulation_id, rules_id, grade, error_report) 
         VALUES ($1, $2, $3, $4, $5) RETURNING id`,
        [session_id, regulation_id || 1, rules_id || 1, grade, error_report]
    );
    return result.rows[0].id;
}

async function getAllResults() {
    const result = await pool.query(`
        SELECT gr.*, reg.recipe_name, grules.rule_name 
        FROM GameResults gr
        LEFT JOIN Regulations reg ON gr.regulation_id = reg.id
        LEFT JOIN GameRules grules ON gr.rules_id = grules.id
        ORDER BY gr.timestamp DESC
    `);
    return result.rows;
}

async function getRegulations() {
    const result = await pool.query('SELECT * FROM Regulations WHERE id = 1');
    if (!result.rows[0]) {
        throw new Error('регламент не найден в бд');
    }
    return result.rows[0];
}

module.exports = { getRegulations, getGameRules, saveResult, getAllResults };