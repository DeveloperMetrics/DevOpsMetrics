
var axios = require('axios');
const getConfig = require('probot-config');

module.exports = async (context) => {

  const { owner, repo } = await context.repo();

  console.log(context.repo());
  
  let repo_config = await getConfig(context,'probotmetrics.yml');
  
  var config = {
    method: 'get',
    url: repo_config.github_settings_url,
    headers: {
      'Cookie': 'ARRAffinity=a0e0c7074c47387d9110460567f8cad9199f0fc29dea2a558ac9771a8088f0e7'
    }
  };

  let deployment_frequency_url = repo_config.deployment_frequency_url;
  let meantime_restore_url = repo_config.meantime_restore_url;
  let lead_time_change_url = repo_config.lead_time_change_url;
  let change_failure_rate_url = repo_config.change_failure_rate_url;  

  await axios(config)
    .then(function (response) {
      console.log("hi");
      var as = Object.values(response.data).filter(n => n.repo == repo);

      deployment_frequency_url += "&clientId=" + as[0]["clientId"];
      deployment_frequency_url += "&clientSecret=" + as[0]["clientSecret"];
      deployment_frequency_url += "&owner=" + as[0]["owner"];
      deployment_frequency_url += "&repo=" + as[0]["repo"];
      deployment_frequency_url += "&branch=" + as[0]["branch"];
      deployment_frequency_url += "&workflowName=" + as[0]["workflowName"];
      deployment_frequency_url += "&workflowId=" + as[0]["workflowId"];

      lead_time_change_url += "&clientId=" + as[0]["clientId"];
      lead_time_change_url += "&clientSecret=" + as[0]["clientSecret"];
      lead_time_change_url += "&owner=" + as[0]["owner"];
      lead_time_change_url += "&repo=" + as[0]["repo"];
      lead_time_change_url += "&branch=" + as[0]["branch"];
      lead_time_change_url += "&workflowName=" + as[0]["workflowName"];
      lead_time_change_url += "&workflowId=" + as[0]["workflowId"];

      change_failure_rate_url += "&organization_owner=" + as[0]["owner"];
      change_failure_rate_url += "&project_repo=" + as[0]["repo"];
      change_failure_rate_url += "&branch=" + as[0]["branch"];
      change_failure_rate_url += "&buildName_workflowName=" + as[0]["workflowName"];

      console.log("deployment_frequency_url");
      console.log(deployment_frequency_url);

      console.log("meantime_restore_url");
      console.log(meantime_restore_url);

      console.log("lead_time_change_url");
      console.log(lead_time_change_url);

      console.log("change_failure_rate_url");
      console.log(change_failure_rate_url);

    })
    .catch(function (error) {
        console.log(error);      
    });

    return {deployment_frequency_url, meantime_restore_url, lead_time_change_url, change_failure_rate_url};

};
