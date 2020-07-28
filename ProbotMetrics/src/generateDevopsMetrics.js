var axios = require('axios');
const prepareDevOpsMetrics  = require('./prepareDevOpsMetrics');

const generateDevopsMetrics = (context) => {

    var config = {
        method: 'get',
        url: 'https://devopsmetrics-prod-eu-service.azurewebsites.net/api/DeploymentFrequency/GetGitHubDeploymentFrequency?getSampleData=False&clientId=68d33f362962599beb85&clientSecret=78e9de5de016d2213de7031712bbfb627e1616f2&owner=samsmithnz&repo=DevOpsMetrics&branch=master&workflowName=DevOpsMetrics%20CI/CD&workflowId=1162561&numberOfDays=30&maxNumberOfItems=20&useCache=True',
        headers: {
            'Cookie': 'ARRAffinity=a0e0c7074c47387d9110460567f8cad9199f0fc29dea2a558ac9771a8088f0e7'
        }
    };

    axios(config)
        .then(function (response) {
            prepareDevOpsMetrics(context, response.data);
        })
        .catch(function (error) {
            console.log(error);
        });
};

module.exports = {
    generateDevopsMetrics,
};
