# DevOps Metrics

[![Build](https://GitHub.com/samsmithnz/DevOpsMetrics/workflows/CI/CD/badge.svg)](https://GitHub.com/samsmithnz/DevOpsMetrics/actions?query=workflow%3ACI%2FCD)

This project is focused on collecting and analyzing high performing DevOps metrics from GitHub and Azure DevOps. [DORA's "State of DevOps" research](https://services.google.com/fh/files/misc/state-of-devops-2019.pdf) and [Accelerate](https://www.amazon.com/Accelerate-Software-Performing-Technology-Organizations/dp/1942788339) highlighted four driving indicators of high performing DevOps teams. While these four metrics are widely used in DevOps discussion, there haven't been many efforts to date to implement and capture the metrics.

- **Deployment frequency**: Number of deployments to production
- **Lead time for changes**: Time from committing a change to deployment to production
- **Mean time to restore (MTTR)**: How quickly restoration of production occurs in an outage or degradation
- **Change failure rate**: After a production deployment, was it successful? Or was a fix or rollback required after the fact?

![High performing metrics](https://samlearnsazure.files.wordpress.com/2020/04/01highperformers.png)
(Chart from page 18 of https://services.google.com/fh/files/misc/state-of-devops-2019.pdf)
A [demo website displaying the metrics can be viewed here](https://devopsmetrics-prod-eu-web.azurewebsites.net/).
More information about high performing DevOps metrics can be found in a blog post here: https://samlearnsazure.blog/2020/04/30/high-performing-devops-metrics/

## The current solution:
**We currently have all four of the metrics implemented and undergoing a pilot. There is a Probot for GitHub. The Azure DevOps widget is still TBD.**

- **Deployment Frequency**, in both Azure DevOps and GitHub:
  - How does it work? We look at the number of successful pipeline runs. 
  - Assumptions/things we can't currently measure: 
      - The build is multi-stage, and leads to a deployment in a production environment.
      - We only look at a single branch (usually the main branch), hence we ignore feature branches (as these probably aren't deploying to production')
  - Current limitations: Only one build/run/branch can be specified
![Deployment Frequency](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/DeploymentFrequencyDemo.png)

- **Lead time for changes**, in both Azure DevOps and GitHub:
  - How does it work? We look at the number of successful pipeline runs and match it with Pull Requests 
  - Assumptions/things we can't currently measure:
      - We currently count the pull request and deployment durations, averaging them for the time period to create the lead time for changes metric.
      - We start measuring at the first commit for a branch. Development is variable that depends on the task, and doesn't help with this measurement.
      - We assume we are following a git flow process, creating feature branches and merging back to the main branch, which is deployed to production on the completion of pull requests
      - We assume that the user requires pull requests to merge work into the main branch - we are looking at all work that is not on this main branch - hence we currently only support one main branch.
  - Current limitations: Only one repo and main branch can be specified
![Lead time for changes](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/LeadTimeForChanges.png)

- **Time to restore service**, in Azure
  - How does it work? We setup Azure Monitor alerts on our resources, for example, on our web service, where we have an alerts for HTTP500 and HTTP403 errors, as well as monitoring CPU and RAM. If any of these alerts are triggered, we capture the alert in an Azure function, and save it into a Azure table storage, where we can aggregate and measure the time of the outage. When the alert is later resolved, this also triggers through the same workflow to save the the resolution and record the restoration of service. 
  - Assumptions/things we can't currently measure:
      - Our project is hosted in Azure
      - The production environment is contained in a single resource group
      - There are appropriate alerts setup on each of the resources, each with action groups to save the alert to Azure Storage 
      - We generate an SLA, but it's entirely based on the MTTR time - assuming the application is "not available" during this time
  - Current limitations: 
      - Only one production resource group can be specified
      - If there is catastrophic resource group failure, (e.g. deleted), there is a high chance that some/all of the alerts will also be deleted
![Time to restore service](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/TimeToRestoreService.png)

- **Change failure rate**, in Azure DevOps and GitHub
  - How does it work? We look at builds, and let the user indicate if it was successful or a failure. By default (currently), the build is considered a failure. (We are going to change this to success by default later) 
  - Assumptions/things we can't currently measure:
      - The build is multi-stage, and leads to a deployment in a production environment.
      - We only look at a single branch (usually the main branch), hence we ignore feature branches (as these probably aren't deploying to production)
      - The user has reviewed the build/deployment and confirmed that the production deployment was successful
  - Current limitations: Only one build/run can be specified
![Change failure rate](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/ChangeFailureRate.png)

# Architecture
Uses .Net CORE 3.1 & MSTest. A GitHub action runs the CI/CD process. 

Currently the CI/CD process: 
1. Builds the code
2. Runs the unit tests
3. Deploys the Probot code to a Azure web app (http://devopsmetrics-prod-eu-probot.azurewebsites.net/)
3. Deploys the webservice to a Azure web app (https://devopsmetrics-prod-eu-service.azurewebsites.net)
4. Deploys the demo website to a Azure web app (https://devopsmetrics-prod-eu-web.azurewebsites.net)

Dependabot runs daily to check for dependency upgrades, and will automatically create a pull request, and approve/close it if all of the tests pass successfully 

![Architecture diagram](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/Architecture.png)


# What's next?
- Upgrades to packaging and setup
- Upgrades to store data in CosmosDB (currently in Azure storage)
- Azure DevOps marketplace integrations, so you can see the changes real time on your project/repo.
- Reviewing the current GitHub probot approach, to find a better target than issues (perhaps a metrics readme.md file?)
- Secret management and integration with Azure Key Vault and/or GitHub Secrets
- Support for more scenarios, releases, etc
- Badges! The API can generate  a URL to these static badges, but more work is needed. Some samples are shown below:

  [![Build](https://img.shields.io/badge/Deployment%20frequency-Elite-brightgreen)](https://img.shields.io/badge/Deployment%20frequency-Elite-brightgreen) [![Build](https://img.shields.io/badge/Lead%20time%20for%20changes-High-green)](https://img.shields.io/badge/Lead%20time%20for%20changes-High-green) [![Build](https://img.shields.io/badge/Time%20to%20restore%20service-Medium-orange)](https://img.shields.io/badge/Time%20to%20restore%20service-Medium-orange) [![Build](https://img.shields.io/badge/Change%20failure%20rate-Low-red)](https://img.shields.io/badge/Change%20failure%20rate-Low-red)

# References

- Azure DevOps API: https://docs.microsoft.com/en-us/rest/api/azure/devops/build/builds/list?view=azure-devops-rest-5.1
- GitHub API: https://developer.GitHub.com/v3/actions/workflow-runs/
