# DevOpsMetrics
A project to experiment with high performing metrics

All four of these metrics are based on production environments, where the value to our end users is delivered:

- Lead time for changes: Time from committing a change to deployment to production
- Deployment frequency: How often we deploy to production
- Mean time to restore(MTTR): How quickly we can restore production in an outage or degradation
- Change failure rate: after a production deployment, was it successful? Or did we need to deploy a fix/rollback?

More information in a blog post here: https://samlearnsazure.blog/2020/04/30/high-performing-devops-metrics/

## The current solution:
- Deployment Frequency, in both Azure DevOps and GitHub:
  - How does it work? We look at the number of successful pipeline runs. 
  - Assumptions/things we can't currently measure: 
    - The deployment is to a production environment
![Deployment Frequency](https://github.com/samsmithnz/DevOpsMetrics/blob/master/ReadmeImages/DeploymentFrequencyDemo.png)

- Lead time for changes, in both Azure DevOps and GitHub:
  - How does it work? We look at the number of successful pipeline runs and match it with Pull Requests 
  - Assumptions/things we can't currently measure: 
    - The project is using a git flow with Pull Requests. 
    - We are measuring the commit duration time in the pull requests. 
    - We measure the time between commits in the Pull Request. If the duration between commits is 5 days, the lead time for changes is 5 days. We may need to consider the wait time for policies/reviews/etc in Pull Requests too. 
    - Commits force-pushed to master are not currently captured. The build deploys to production
[No screenshot yet]

# Architecture
Uses .Net CORE 3.1 & MSTest. A GitHub action runs the CI/CD process. 

[![Build](https://github.com/samsmithnz/DevOpsMetrics/workflows/CI/CD/badge.svg)](https://github.com/samsmithnz/DevOpsMetrics/actions?query=workflow%3ACI%2FCD)

Currently the CI/CD process: 
1. Builds the code
2. Runs the unit tests
3. Deploys the webservice to a single/prod Azure web app (https://devopsmetrics-prod-eu-service.azurewebsites.net)
4. Deploys the demo website to a single/prod Azure web app (https://devopsmetrics-prod-eu-web.azurewebsites.net)

Dependabot runs daily to check for dependency upgrades, and will automatically create a pull request, and approve/close it if all of the tests pass successfully 

# References

- Azure DevOps API: https://docs.microsoft.com/en-us/rest/api/azure/devops/build/builds/list?view=azure-devops-rest-5.1
- GitHub API: https://developer.github.com/v3/actions/workflow-runs/