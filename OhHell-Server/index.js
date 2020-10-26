const chalk = require('chalk');
const bodyParser = require('body-parser');
const cors = require('cors');
const debug = require('debug')('ohhell-server');
const express = require('express');
const path = require('path');// Load configuration
const gameRouter = require('./source/routes.js');

global.appRoot = path.resolve(__dirname);

const app = express();
app.use(bodyParser.json()); // Set up express to use json parser.
app.use(cors()); // Enable cross-origin resource sharing (web security thing).// Set up routing.
app.use('/game', gameRouter);
app.use(express.static('./static_content'));
const listenPort = 8082;
app.listen(listenPort, () => {
  debug(`Listening on port ${chalk.green(listenPort)}`);
});
