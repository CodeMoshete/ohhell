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

module.exports.createGame = function createGame(gameState) {
  const gameStateName = `gameState-${gameState.GameName}`;
  const gameStateBasePath = path.join(global.appRoot, 'gamestates');
  const gameStatePath = path.join(gameStateBasePath, gameStateName);

  if (!fs.existsSync(gameStateBasePath)) {
    fs.mkdirSync(gameStateBasePath, { recursive: true });
  }

  if (!fs.existsSync(gameStatePath)) {
    const response = JSON.stringify(gameState, null, 2);
    fs.writeFileSync(gameStatePath, response);
    return response;
  }
  return false;
};

module.exports.joinGame = function joinGame(gameName, playerName) {
  const gameStateName = `gameState-${gameName}`;
  const gameStatePath = path.join(global.appRoot, 'gamestates', gameStateName);
  if (fs.existsSync(gameStatePath)) {
    const gameStateContent = JSON.parse(fs.readFileSync(gameStatePath));
    const playerList = gameStateContent.Players;
    let hasPlayer = false;
    for (let i = 0, count = playerList.length; i < count; i += 1) {
      if (playerList[i].PlayerName === playerName) {
        hasPlayer = true;
        break;
      }
    }

    if (!hasPlayer) {
      playerList.push({
        PlayerName: playerName,
        IsHost: false,
        CurrentHand: [],
        CurrentBid: 0,
        CurrentTricks: 0,
        Bids: [],
        Tricks: []
      });
    }

    const serializedContent = JSON.stringify(gameStateContent, null, 2);
    fs.writeFileSync(gameStatePath, serializedContent);
    return serializedContent;
  }
  return false;
};

module.exports.getGameState = function getGameState(gameName) {
  const gameStateName = `gameState-${gameName}`;
  const gameStateBasePath = path.join(global.appRoot, 'gamestates');
  const gameStateFilePath = path.join(gameStateBasePath, gameStateName);
  let gameStateContent = false;
  if (fs.existsSync(gameStateFilePath)) {
    gameStateContent = fs.readFileSync(gameStateFilePath);
  }
  debug(`GET GAME STATE: ${gameStateFilePath}`);
  return gameStateContent;
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

  const enforcedIndex = actionData.EnforceIndex;
  const currentIndex = actionContent.length;
  if (enforcedIndex >= 0 && currentIndex !== enforcedIndex) {
    debug(`Current action index ${currentIndex} does not equal enforced index ${enforcedIndex}`);
    return;
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
  }

  const actionTypes = [];
  const actionDatas = [];
  const numActions = actionContent.length;
  const intStartIndex = parseInt(startIndex, 10);
  if (numActions > intStartIndex) {
    for (let i = intStartIndex; i < numActions; i += 1) {
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

  const response = JSON.stringify(actionsRecord);
  debug(`ACTION UPDATE:\n${JSON.stringify(actionsRecord, null, 2)}`);
  return response;
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

module.exports.getGamesListSimple = function getGamesListSimple() {
  const allGamesData = [];
  const gameStateBasePath = path.join(global.appRoot, 'gamestates');
  if (fs.existsSync(gameStateBasePath)) {
    const allGames = fs.readdirSync(gameStateBasePath);
    for (let i = 0, count = allGames.length; i < count; i += 1) {
      if (allGames[i].startsWith('gameState')) {
        const gameData = JSON.parse(fs.readFileSync(path.join(gameStateBasePath, allGames[i])));
        const returnData = {
          gameName: gameData.GameName,
          playerCount: gameData.Players.length,
          isLaunched: gameData.IsLaunched,
          isFinished: gameData.IsFinished
        };
        allGamesData.push(returnData);
      }
    }
  }
  const returnData = {
    ActiveGames: allGamesData
  };
  debug(`Found ${allGamesData.length} games`);
  return JSON.stringify(returnData);
};

module.exports.deleteGame = function deleteGame(gameName) {
  const gameStateBasePath = path.join(global.appRoot, 'gamestates');
  const gameDataPath = path.join(gameStateBasePath, `gameState-${gameName}`);
  if (fs.existsSync(gameDataPath)) {
    fs.unlinkSync(gameDataPath);
    debug(`Deleted ${gameDataPath}`);
  }

  const gameActionsPath = path.join(gameStateBasePath, `gameActions-${gameName}`);
  if (fs.existsSync(gameActionsPath)) {
    fs.unlinkSync(gameActionsPath);
    debug(`Deleted ${gameActionsPath}`);
  }
};
