var axios = require('axios');
const prepareDevOpsMetrics = require('./prepareDevOpsMetrics');
const getGithubSettings = require('./getGithubSettings');

const generateDevopsMetrics = async (context) => {

    let urls = await getGithubSettings(context);

    console.log('generateDevopsMetrics');
    console.log(urls);

    let deployment_frequency_data;
    let meantime_restore_data;
    let lead_time_change_data;
    let change_failure_rate_data;

    var deployment_frequency_config = {
        method: 'get',
        url: encodeURI(urls.deployment_frequency_url),
        headers: {
            'Cookie': 'ARRAffinity=a0e0c7074c47387d9110460567f8cad9199f0fc29dea2a558ac9771a8088f0e7'
        }
    };

    await axios(deployment_frequency_config)
        .then(function (response) {
            deployment_frequency_data = response.data;
        })
        .catch(function (error) {
            console.log(error);
        });

        var meantime_restore_config = {
            method: 'get',
            url: encodeURI(urls.meantime_restore_url),
            headers: {
                'Cookie': 'ARRAffinity=a0e0c7074c47387d9110460567f8cad9199f0fc29dea2a558ac9771a8088f0e7'
            }
        };
    
        await axios(meantime_restore_config)
            .then(function (response) {
                meantime_restore_data = response.data;
            })
            .catch(function (error) {
                console.log(error);
            });

            var lead_time_change_config = {
                method: 'get',
                url: encodeURI(urls.lead_time_change_url),
                headers: {
                    'Cookie': 'ARRAffinity=a0e0c7074c47387d9110460567f8cad9199f0fc29dea2a558ac9771a8088f0e7'
                }
            };
        
            await axios(lead_time_change_config)
                .then(function (response) {
                    lead_time_change_data = response.data;
                })
                .catch(function (error) {
                    console.log(error);
                });

                var change_failure_rate_config = {
                    method: 'get',
                    url: encodeURI(urls.change_failure_rate_url),
                    headers: {
                        'Cookie': 'ARRAffinity=a0e0c7074c47387d9110460567f8cad9199f0fc29dea2a558ac9771a8088f0e7'
                    }
                };
            
                await axios(change_failure_rate_config)
                    .then(function (response) {
                        change_failure_rate_data = response.data;
                    })
                    .catch(function (error) {
                        console.log(error);
                    });

            prepareDevOpsMetrics(context, deployment_frequency_data , meantime_restore_data, lead_time_change_data,change_failure_rate_data);

};


module.exports = {
    generateDevopsMetrics,
};
