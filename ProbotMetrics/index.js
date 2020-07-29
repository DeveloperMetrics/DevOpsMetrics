/**
 * This is the main entrypoint to your Probot app
 * @param {import('probot').Application} app
 */

var fs = require('fs');
var appRoot = require('app-root-path');

const createScheduler = require('probot-scheduler')
const { generateDevopsMetrics } = require('./src/generateDevopsMetrics');


module.exports = (app) => {
  // Your code here
  app.log('Yay, the app was loaded!')

  const dir = process.cwd(); 
  
  let settings = JSON.parse(fs.readFileSync(dir + '/settings.json', encoding = "ascii"));

  createScheduler(app, {
    delay: !!process.env.DISABLE_DELAY, // delay is enabled on first run
    interval: settings["hours_interval"] * settings["minutes_interval"] * settings["seconds_interval"] * 1000
  })

  app.on('schedule.repository', async context => {
   
    app.log('Application is running per schedule');
    context.log.debug('App is running as per schedule.repository');

    await generateDevopsMetrics(context);
    
  })
  
}



