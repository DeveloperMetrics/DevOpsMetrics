
const moment = require('moment');
const postDevOpsMetrics = require('./postDevOpsMetrics');
const getConfig = require('probot-config');

module.exports = async (context, deployment_frequency_data, meantime_restore_data, lead_time_change_data,change_failure_rate_data) => {
  console.log(JSON.stringify(deployment_frequency_data));
  console.log(JSON.stringify(meantime_restore_data));
  const { owner, repo } = context.repo();

  const title = 'Daily DevOps Metrics for ' + moment().format('DD-MMM-YYYY');  
  
  let repo_config = await getConfig(context,'probotmetrics.yml');

  let body = "";

  body += '## Daily DevOps Metrics for ' + moment().format('DD-MMM-YYYY'); 
  body += "\n<br/> [![Deployment frequency]("+ deployment_frequency_data["badgeWithMetricURL"] +")](" + repo_config.dashboard_url + owner + "_" + repo + ")";
  body += "\n<br/> [![Lead time for changes]("+ lead_time_change_data["badgeWithMetricURL"] +")](" + repo_config.dashboard_url + owner + "_" + repo + ")";
  body += "\n<br/> [![Time to restore service](" + meantime_restore_data["badgeWithMetricURL"] + ")](" + repo_config.dashboard_url + owner + "_" + repo + ")";
  body += "\n<br/> [![Change failure rate]("+ change_failure_rate_data["badgeWithMetricURL"] +")](" + repo_config.dashboard_url + owner + "_" + repo + ")";

  body += "\n<br/> **Note:** metrics based on data from the last 30 days";
  body += "\n<br/> _This issue was created with the DevOps metrics probot. Documentation about the metrics and source code is available at [https://github.com/samsmithnz/devopsmetrics](https://github.com/samsmithnz/devopsmetrics)_";

  const labels = ['daily-devops'];

  await postDevOpsMetrics(context, {
    owner, repo, title, body, labels,
  }); 

};

