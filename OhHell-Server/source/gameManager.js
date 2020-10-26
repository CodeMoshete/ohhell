const debug = require('debug')('ohhell-server');
const path = require('path');
const fs = require('fs');

module.exports.setGameState = function setGameState(gameState) {
  const gameStateName = `gameState-${gameState.GameName}`;
  const gameStateBasePath = path.join(global.appRoot, 'gamestates');

  if (!fs.existsSync(gameStateBasePath)) {
    fs.mkdirSync(gameStateBasePath, { recursive: true });
  }

  const gameStateFilePath = path.join(gameStateBasePath, gameStateName);
  debug(`SET GAME STATE: ${gameStateFilePath}`);
  fs.writeFileSync(gameStateFilePath, JSON.stringify(gameState, null, 2));
};

module.exports.getGameState = function getGameState(gameName) {
  const gameStateName = `gameState-${gameName}`;
  const gameStateBasePath = path.join(global.appRoot, 'gamestates');
  const gameStateFilePath = path.join(gameStateBasePath, gameStateName);
  debug(`GET GAME STATE: ${gameStateFilePath}`);
  return fs.readFileSync(gameStateFilePath);
};

module.exports.getGamesList = function getGamesList() {
  const allGamesData = [];
  const gameStateBasePath = path.join(global.appRoot, 'gamestates');
  if (fs.existsSync(gameStateBasePath)) {
    const allGames = fs.readdirSync(gameStateBasePath);
    for (let i = 0, count = allGames.length; i < count; i += 1) {
      allGamesData.push(JSON.parse(fs.readFileSync(path.join(gameStateBasePath, allGames[i]))));
    }
  }
  const returnData = {
    ActiveGames: allGamesData
  };
  debug(`Found ${allGamesData.length} games`);
  return JSON.stringify(returnData);
};
