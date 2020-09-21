/**
 * This is the main entrypoint to your Probot app
 * @param {import('probot').Application} app
 */

const createScheduler = require('probot-scheduler')
const { generateDevopsMetrics } = require('./src/generateDevopsMetrics');

module.exports = (app) =>  {
  // Your code here
  app.log('Yay, the app was loaded!')

  createScheduler(app, {
    delay: !!process.env.DISABLE_DELAY, // delay is enabled on first run
    interval: 60 * 1000 * 60 * 24 * 7 //weekly
  })

  app.on('schedule.repository', async context => {

    app.log('Application is running per schedule');
    context.log.debug('App is running as per schedule.repository');

    await generateDevopsMetrics(context);

  })

}



