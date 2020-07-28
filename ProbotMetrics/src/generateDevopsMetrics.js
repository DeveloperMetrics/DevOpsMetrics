var axios = require('axios');
const prepareDevOpsMetrics  = require('./prepareDevOpsMetrics');
const getGithubSettings  = require('./getGithubSettings');

const generateDevopsMetrics = async (context) => {

    let url = await getGithubSettings(context);

    console.log(url);

    var config = {
        method: 'get',
        url: encodeURI(url),
        headers: {
            'Cookie': 'ARRAffinity=a0e0c7074c47387d9110460567f8cad9199f0fc29dea2a558ac9771a8088f0e7'
        }
    };

    

   await axios(config)
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
