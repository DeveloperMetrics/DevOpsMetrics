
var axios = require('axios');
const getConfig = require('probot-config');

module.exports = async (context) => {

  const { owner, repo } = await context.repo();

  console.log(context.repo());
  
  let repo_config = await getConfig(context,'probotmetrics.yml');

  console.log(repo_config.github_settings_url);



  var config = {
    method: 'get',
    url: repo_config.github_settings_url,
    headers: {
      'Cookie': 'ARRAffinity=a0e0c7074c47387d9110460567f8cad9199f0fc29dea2a558ac9771a8088f0e7'
    }
  };

  let data_url = repo_config.data_url;

  await axios(config)
    .then(function (response) {

      console.log(response);

      var as = Object.values(response.data).filter(n => n.repo == repo);

      console.log(as);

      data_url += "&clientId=" + as[0]["clientId"];
      data_url += "&clientSecret=" + as[0]["clientSecret"];
      data_url += "&owner=" + as[0]["owner"];
      data_url += "&repo=" + as[0]["repo"];
      data_url += "&branch=" + as[0]["branch"];
      data_url += "&workflowName=" + as[0]["workflowName"];
      data_url += "&workflowId=" + as[0]["workflowId"];

      

    })
    .catch(function (error) {
        console.log(error);      
    });

    return data_url;

};
