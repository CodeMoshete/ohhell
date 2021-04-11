const debug = require('debug')('ohhell-server');
const express = require('express');
const util = require('util');
const gameManager = require('./gameManager.js');
const adminTools = require('./adminTools.js');

const router = express.Router();

router.route('/createGame')
  .post(async (req, res) => {
    debug(`Creating gamestate!\n${util.inspect(req.body)}`);
    const result = await gameManager.createGame(req.body);
    res.send(result);
  });

router.route('/setGameState')
  .post(async (req, res) => {
    debug(`Setting gamestate!\n${util.inspect(req.body)}`);
    await gameManager.setGameState(req.body);
    res.sendStatus(200);
  });

router.route('/joinGame')
  .post(async (req, res) => {
    debug(`Joining game!\n${req.body}`);
    const result = await gameManager.joinGame(req.body.GameName, req.body.PlayerName);
    res.send(result);
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
    const simple = req.query.simple === 'true';
    const gamesList = simple
      ? gameManager.getGamesListSimple(req.query.gameName)
      : gameManager.getGamesList(req.query.gameName);
    res.send(gamesList);
  });

router.route('/addGameAction')
  .post(async (req, res) => {
    debug(`Setting game action!\n${util.inspect(req.body)}`);
    gameManager.logGameAction(req.body);
    res.sendStatus(200);
  });

router.route('/getGameActions')
  .get(async (req, res) => {
    debug('Getting game actions!');
    const gamesList = gameManager.getGameActions(req.query.gameName, req.query.actionIndex);
    res.send(gamesList);
  });

router.route('/deleteGame')
  .post(async (req, res) => {
    debug(`Deleting game!\n${util.inspect(req.body)}`);
    gameManager.deleteGame(req.body.gameName);
    res.sendStatus(200);
  });

router.route('/adjustPlayerBid')
  .get(async (req, res) => {
    const playerId = req.query.id;
    const bidNum = req.query.bid;
    const handNum = req.query.hand;
    const gameName = req.query.game;
    await adminTools.adjustPlayerBid(gameName, playerId, handNum, bidNum);
    res.sendStatus(200);
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
