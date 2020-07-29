

var fs = require('fs');
var axios = require('axios');

module.exports = async (context) => {

  const dir = process.cwd(); 

  let settings = JSON.parse(fs.readFileSync(dir + '/settings.json', encoding = "ascii"));

  const { owner, repo } = await context.repo();
  console.log(context.repo());

  var config = {
    method: 'get',
    url: settings["github_settings_url"],
    headers: {
      'Cookie': 'ARRAffinity=a0e0c7074c47387d9110460567f8cad9199f0fc29dea2a558ac9771a8088f0e7'
    }
  };

  let data_url = settings["data_url"];

  await axios(config)
    .then(function (response) {
      var as = Object.values(response).filter(n => n.repo == repo);

      data_url += "&clientId=" + as["clientId"];
      data_url += "&clientSecret=" + as["clientSecret"];
      data_url += "&owner=" + as["owner"];
      data_url += "&repo=" + as["repo"];
      data_url += "&branch=" + as["branch"];
      data_url += "&workflowName=" + as["workflowName"];
      data_url += "&workflowId=" + as["workflowId"];

    })
    .catch(function (error) {
        console.log(error);      
    });

    return data_url;

};
