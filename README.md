# DevOps Metrics

[![Build](https://GitHub.com/samsmithnz/DevOpsMetrics/workflows/CI/CD/badge.svg)](https://GitHub.com/samsmithnz/DevOpsMetrics/actions?query=workflow%3ACI%2FCD)
[![Coverage Status](https://coveralls.io/repos/github/samsmithnz/DevOpsMetrics/badge.svg?branch=main)](https://coveralls.io/github/samsmithnz/DevOpsMetrics?branch=main)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=samsmithnz_DevOpsMetrics&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=samsmithnz_DevOpsMetrics)
[![Current Release](https://img.shields.io/github/release/samsmithnz/DevOpsMetrics/all.svg)](https://github.com/samsmithnz/DevOpsMetrics/releases)


**Why should we care about DevOps Metrics and what are they?** All engineering, including software, needs metrics to track performance, but many metrics when measured individually, can be 'gamed', or don't encourage the right behaviors or incentives. This has been an issue with metrics for many years. The [DORA metrics](https://services.google.com/fh/files/misc/state-of-devops-2019.pdf) are a step in the right direction, combining several metrics that encourage the behaviors and incentives - and hence that encourage DevOps teams to perform at a high level of performance.  DORA metrics aren't perfect, but are still the best we have available today.
- A [demo website displaying these metrics can be viewed here](https://devops-prod-eu-web.azurewebsites.net/).
- Insights I've noted about implementing DORA DevOps metrics can be found in a [blog post here](https://samlearnsazure.blog/2020/04/30/high-performing-devops-metrics/)

This project is focused on helping you collect and analyze four key high performing DevOps metrics from GitHub and Azure DevOps. [DORA's "State of DevOps" research](https://cloud.google.com/blog/products/devops-sre/announcing-dora-2021-accelerate-state-of-devops-report) and [Accelerate](https://www.amazon.com/Accelerate-Software-Performing-Technology-Organizations/dp/1942788339) highlighted four driving indicators of high performing DevOps teams. While these four metrics are widely used in DevOps discussion, it's challenging to implement and capture all of the metrics.

- **Deployment frequency: Number of deployments to production.** This is important, as it highlights how often you can deploy to production - which in turn indicates there is a mature automated testing and a mature CI/CD pipeline to release to production.
- **Lead time for changes: Time from committing a change to deployment to production.** How quickly can we change a line of code and have it running in production? Again, this indicates mature automated testing and a mature CI/CD pipeline able to handle changes.
- **Mean time to restore (MTTR): How quickly restoration of production occurs in an outage or degradation.** When there is a degradation, how quickly can the system auto-heal itself, scale to handle increased load, and/or This one is contraversal, as it's challenging to compare different events that cause degradation. 
- **Change failure rate: After a production deployment, was it successful? Or was a fix or rollback required after the fact?** How often is a change we made 'successful'? This ties in well with deployment frequency and lead time for changes, but is challenging to measure - as it requires a signoff off of success. Not just that the code deployed correctly, but that there weren't adverse effects or degradation of the deployment to the system

![High performing metrics](https://user-images.githubusercontent.com/8389039/212061370-6984b2c3-bc13-4d92-8afc-0068be4cdde1.png)
[^1]

## The current solution:
**We currently have all four of the metrics implemented and undergoing a pilot. (The Azure DevOps widget is currently not planned - but is possible if someone wants to build it!).**

- **Deployment Frequency**, in both Azure DevOps and GitHub:
  - How does it work? We look at the number of successful pipeline runs. 
  - Assumptions/things we can't currently measure: 
      - The build is multi-stage, and leads to a deployment in a production environment.
      - We only look at a single branch (usually the main branch), hence we ignore feature branches (as these probably aren't deploying to production)
  - Current limitations: Only one build/run/branch can be specified
![Deployment Frequency](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/DeploymentFrequencyDemo.png)

- **Lead time for changes**, in both Azure DevOps and GitHub:
  - How does it work? We look at the number of successful pipeline runs and match it with Pull Requests 
  - Assumptions/things we can't currently measure:
      - We currently count the pull request and deployment durations, averaging them for the time period to create the lead time for changes metric.
      - We start measuring at the last commit for a branch to the PR close/merge time. Development is variable that depends on the task, and doesn't help with this measurement.
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
Developed in .NET 8. A GitHub action runs the CI/CD process. 

Currently the CI/CD process: 
1. Builds the code
2. Runs the unit tests
3. Deploys the webservice to a Azure web app (https://devops-prod-eu-service.azurewebsites.net)
4. Deploys the demo website to a Azure web app (https://devops-prod-eu-web.azurewebsites.net)
4. Deploys the function website to a Azure function 

Dependabot runs daily to check for dependency upgrades. 

![Architecture diagram](https://github.com/samsmithnz/DevOpsMetrics/blob/main/ReadmeImages/Architecture.png)

## Badges
The API can generate a URL for static badges, some samples are shown below:
[![Build](https://img.shields.io/badge/Lead%20time%20for%20changes-High-green)](https://img.shields.io/badge/Lead%20time%20for%20changes-High-green) [![Build](https://img.shields.io/badge/Time%20to%20restore%20service-Medium-orange)](https://img.shields.io/badge/Time%20to%20restore%20service-Medium-orange) [![Build](https://img.shields.io/badge/Change%20failure%20rate-Low-red)](https://img.shields.io/badge/Change%20failure%20rate-Low-red)

# Setup

## Deploying to Azure

- Run the infrastructure setup script [Currently \src\DevOpsMetrics.Infrastructure\DeployInfrastructureToAzure2.ps1]
- DevOpsMetrics.Service setup: Keyvault URL and application insights id set as part of setup script
- Browse to [website name].azurewebsites.net/Home/Settings, and setup your projects as needed. Note that all secrets are loaded into the keyvault and are controlled by you!

## To debug/run tests

# What's next?
- Upgrades to packaging and setup (in progress)
- Upgrades to store data in CosmosDB (currently in Azure storage)
- Support for more scenarios, releases, etc
- ~~Azure DevOps marketplace integrations, so you can see the changes real time on your project/repo.~~ (lower priority to focus on GitHub)

# References

- GitHub API: https://developer.GitHub.com/v3/actions/workflow-runs/
- Azure DevOps API: https://docs.microsoft.com/en-us/rest/api/azure/devops/build/builds/list?view=azure-devops-rest-5.1

[^1]: Chart from [page 11 of state of DevOps 2022 report](https://cloud.google.com/devops/state-of-devops)