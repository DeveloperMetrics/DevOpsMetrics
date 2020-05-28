# DevOpsMetrics
A project to experiment with high performing metrics

All four of these metrics are based on production environments, where the value to our end users is delivered:

- Lead time for changes: Time from committing a change to deployment to production
- Deployment frequency: How often we deploy to production
- Mean time to restore(MTTR): How quickly we can restore production in an outage or degradation
- Change failure rate: after a production deployment, was it successful? Or did we need to deploy a fix/rollback?

![High performing metrics](https://samlearnsazure.files.wordpress.com/2020/04/01highperformers.png)

More information in a blog post here: https://samlearnsazure.blog/2020/04/30/high-performing-devops-metrics/

## The current solution:
We currently have 3 of the 4 metrics partly implemented, and undergoing the pilot
- **Deployment Frequency**, in both Azure DevOps and GitHub:
  - How does it work? We look at the number of successful pipeline runs. 
  - Assumptions/things we can't currently measure: 
      - The build is multi-stage, and leads to a deployment in a production environment.
      - We only look at a single branch (usually the master branch), hence we ignore feature branches (as these probably aren't deploying to production')
![Deployment Frequency](https://github.com/samsmithnz/DevOpsMetrics/blob/master/ReadmeImages/DeploymentFrequencyDemo.png)

- **Lead time for changes**, in both Azure DevOps and GitHub:
  - How does it work? We look at the number of successful pipeline runs and match it with Pull Requests 
  - Assumptions/things we can't currently measure:
      - We currently count the pull request and deployment durations, averaging them for the time period to create the lead time for changes metric.
      - We start measuring at the first commit for a branch. Development is variable that depends on the task, and doesn't help with this measurement.
      - We assume we are following a git flow process, creating feature branches and merging back to the master branch, which is deployed to production on the completion of pull requests
      - We assume that the user requires pull requests to merge work into the master branch - we are looking at all work that is not on this master branch - hence we currently only support one master branch.
![Lead time for changes](https://github.com/samsmithnz/DevOpsMetrics/blob/master/ReadmeImages/LeadTimeForChanges.png)

- **Time to restore service**, in Azure
  - How does it work? We setup Azure Monitor alerts on our resources, for example, on our web service, where we have an alerts for HTTP500 and HTTP403 errors, as well as monitoring CPU and RAM. If any of these alerts are triggered, we capture the alert in an Azure function, and save it into a Azure table storage, where we can aggregate and measure the time of the outage. When the alert is later resolved, this also triggers through the same workflow to save the the resolution and record the restoration of service. <br />
  - Assumptions/things we can't currently measure:
      - Our project is hosted in Azure
      - The production environment is contained in a single resource group
      - There are appropriate alerts setup on each of the resources, each with action groups to save the alert to Azure Storage <br />
![Time to restore service](https://github.com/samsmithnz/DevOpsMetrics/blob/master/ReadmeImages/TimeToRestoreService.png)

# Architecture
Uses .Net CORE 3.1 & MSTest. A GitHub action runs the CI/CD process. 

[![Build](https://GitHub.com/samsmithnz/DevOpsMetrics/workflows/CI/CD/badge.svg)](https://GitHub.com/samsmithnz/DevOpsMetrics/actions?query=workflow%3ACI%2FCD)

Currently the CI/CD process: 
1. Builds the code
2. Runs the unit tests
3. Deploys the webservice to a single/prod Azure web app (https://devopsmetrics-prod-eu-service.azurewebsites.net)
4. Deploys the demo website to a single/prod Azure web app (https://devopsmetrics-prod-eu-web.azurewebsites.net)

Dependabot runs daily to check for dependency upgrades, and will automatically create a pull request, and approve/close it if all of the tests pass successfully 

# References

- Azure DevOps API: https://docs.microsoft.com/en-us/rest/api/azure/devops/build/builds/list?view=azure-devops-rest-5.1
- GitHub API: https://developer.GitHub.com/v3/actions/workflow-runs/