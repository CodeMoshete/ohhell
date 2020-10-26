const debug = require('debug')('ohhell-server');
const express = require('express');
const util = require('util');
const gameManager = require('./gameManager.js');

const router = express.Router();

router.route('/setGameState')
  .post(async (req, res) => {
    debug(`Setting gamestate!\n${util.inspect(req.body)}`);
    gameManager.setGameState(req.body);
    res.sendStatus(200);
  });

router.route('/getGameState')
  .get(async (req, res) => {
    debug('Getting game state!');
    const gameState = gameManager.getGameState(req.query.gameName);
    res.send(gameState);
  });

router.route('/getGamesList')
  .get(async (req, res) => {
    debug('Getting game state!');
    const gamesList = gameManager.getGamesList(req.query.gameName);
    res.send(gamesList);
  });

router.route('/')
  .get(async (req, res) => {
    res.send('Game server running!');
  });

router.route('/admin')
  .get(async (req, res) => {
    res.send('TODO: Admin Panel');
  });

module.exports = router;
