
const moment = require('moment');
const postDevOpsMetrics = require('./postDevOpsMetrics');

module.exports = async (context, data) => {
  console.log(JSON.stringify(data));
  const { owner, repo } = context.repo();

  const title = '##Daily DevOps Metrics for ' + moment().format('DD-MMM-YYYY, h:mm:ss a');    

  let body = "";

  body += "## Daily DevOps Metrics"; 
  body += "\n<br/> Days Included In Metrics : " + data["numberOfDays"]
  body += "\n<br/> ![Devops Badge](" + data["badgeURL"] + ")";

  body += "\n | Metric | Value |\n";
  body += "| --- | --- |\n";
  body += "| Deployments Per Day Metric | " + data["deploymentsPerDayMetric"] + "|\n";
  body += "| Deployments " + data["deploymentsToDisplayUnit"] + " | " + data["deploymentsToDisplayMetric"] + "|\n";
  
  body += "\n";
  body += "## Build History\n"; 

  body += "| buildNumber | startTime | endTime | buildDurationInMinutesAndSeconds | status |\n";
  body += "| --- | --- | --- | --- | --- |\n";

  for ( var i = 0; i < data["buildList"].length; i++) {
    body += "|"  + data["buildList"][i]["buildNumber"]; 
    body += "|"  + moment(data["buildList"][i]["startTime"]).format('DD-MMM-YYYY, h:mm:ss a'); 
    body += "|"  + moment(data["buildList"][i]["endTime"]).format('DD-MMM-YYYY, h:mm:ss a'); 
    body += "|"  + data["buildList"][i]["buildDurationInMinutesAndSeconds"]; 
    body += "|"  + data["buildList"][i]["status"]  + "|\n"; 
  };

  const labels = ['daiy-devops'];

  await postDevOpsMetrics(context, {
    owner, repo, title, body, labels,
  }); 

};
