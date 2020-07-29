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

  let repo_config = await context.config( 'probotmetrics.yml');

  createScheduler(app, {
    delay: !!process.env.DISABLE_DELAY, // delay is enabled on first run
    interval: repo_config.hours_interval * repo_config.minutes_interval * repo_config.seconds_interval * 1000
  })

  app.on('schedule.repository', async context => {
   
    app.log('Application is running per schedule');
    context.log.debug('App is running as per schedule.repository');

    await generateDevopsMetrics(context);
    
  })
  
}



