# DevOpsMetrics
A project to experiment with high performing metrics

All four of these metrics are based on production environments, where the value to our end users is delivered:

- Lead time for changes: Time from committing a change to deployment to production
- Deployment frequency: How often we deploy to production
- Mean time to restore(MTTR): How quickly we can restore production in an outage or degradation
- Change failure rate: after a production deployment, was it successful? Or did we need to deploy a fix/rollback?

More information in a blog post here: https://samlearnsazure.blog/2020/04/30/high-performing-devops-metrics/

The current solution shows:
- Deployment Frequency, in both Azure DevOps and GitHub:
![Deployment Frequency](https://github.com/samsmithnz/DevOpsMetrics/blob/master/ReadmeImages/DeploymentFrequencyDemo.png)


# Architecture
Uses .Net CORE 3.1, MSTest. A GitHub action runs the CI/CD process. 

[![Build](https://github.com/samsmithnz/DevOpsMetrics/workflows/CI/CD/badge.svg)](https://github.com/samsmithnz/DevOpsMetrics/actions?query=workflow%3ACI%2FCD)

Currently the CI/CD process: 
1. builds the code
2. runs the unit tests
3. deploys the webservice to Azure (https://devopsmetrics-prod-eu-service.azurewebsites.net)
4. deploys the demo website to Azure (https://devopsmetrics-prod-eu-web.azurewebsites.net)

Dependabot runs daily to check for dependency upgrades, and will automatically create a pull request, and approve/close it if all of the tests pass successfully 

# References

- Azure DevOps API: https://docs.microsoft.com/en-us/rest/api/azure/devops/build/builds/list?view=azure-devops-rest-5.1
- GitHub API: https://developer.github.com/v3/actions/workflow-runs/