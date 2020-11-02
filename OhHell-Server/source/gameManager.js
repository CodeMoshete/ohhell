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

module.exports.logGameAction = function logGameAction(actionData) {
  const gameActionName = `gameActions-${actionData.GameName}`;
  const gameActionBasePath = path.join(global.appRoot, 'gamestates');
  const gameActionFilePath = path.join(gameActionBasePath, gameActionName);

  if (!fs.existsSync(gameActionBasePath)) {
    fs.mkdirSync(gameActionBasePath, { recursive: true });
  }

  let actionContent = [];
  if (fs.existsSync(gameActionFilePath)) {
    actionContent = JSON.parse(fs.readFileSync(gameActionFilePath));
  }
  actionContent.push(actionData);

  debug(`LOG GAME ACTION: ${gameActionFilePath}`);
  fs.writeFileSync(gameActionFilePath, JSON.stringify(actionContent, null, 2));
};

module.exports.getGameActions = function getGameActions(gameName, startIndex) {
  const gameActionName = `gameActions-${gameName}`;
  const gameActionBasePath = path.join(global.appRoot, 'gamestates');
  const gameActionFilePath = path.join(gameActionBasePath, gameActionName);

  let actionContent = [];
  if (fs.existsSync(gameActionFilePath)) {
    actionContent = JSON.parse(fs.readFileSync(gameActionFilePath));
    debug(`READING ACTIONS:\n${JSON.stringify(actionContent, null, 2)}`);
  }

  const actionTypes = [];
  const actionDatas = [];
  const numActions = actionContent.length;
  debug(`START INDEX: ${startIndex}`);
  if (numActions > startIndex) {
    for (let i = startIndex; i < numActions; i += 1) {
      const action = actionContent[i];
      actionTypes.push(action.ActionType);
      actionDatas.push(action.ActionData);
    }
  }

  const actionsRecord = {
    GameName: gameName,
    ActionIndex: numActions,
    ActionTypes: actionTypes,
    ActionDatas: actionDatas
  };

  return JSON.stringify(actionsRecord);
};

module.exports.getGamesList = function getGamesList() {
  const allGamesData = [];
  const gameStateBasePath = path.join(global.appRoot, 'gamestates');
  if (fs.existsSync(gameStateBasePath)) {
    const allGames = fs.readdirSync(gameStateBasePath);
    for (let i = 0, count = allGames.length; i < count; i += 1) {
      if (allGames[i].startsWith('gameState')) {
        allGamesData.push(JSON.parse(fs.readFileSync(path.join(gameStateBasePath, allGames[i]))));
      }
    }
  }
  const returnData = {
    ActiveGames: allGamesData
  };
  debug(`Found ${allGamesData.length} games`);
  return JSON.stringify(returnData);
};
