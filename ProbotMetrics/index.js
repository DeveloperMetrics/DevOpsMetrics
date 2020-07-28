/**
 * This is the main entrypoint to your Probot app
 * @param {import('probot').Application} app
 */
const createScheduler = require('probot-scheduler')
const { generateDevopsMetrics } = require('./src/generateDevopsMetrics');


module.exports = (app) => {
  // Your code here
  app.log('Yay, the app was loaded!')

  createScheduler(app, {
    delay: !!process.env.DISABLE_DELAY, // delay is enabled on first run
    interval: 1000 * 60 * 60 * 12 //1000 ms * 60 seconds * 60 minutes * 12 hours (Run every 12 hours) 
  })

  app.on('schedule.repository', context => {
   
    app.log('Application is running per schedule');
    context.log.debug('App is running as per schedule.repository');

    generateDevopsMetrics(context);
    
  })
  
}



