# ProbotMetrics

> A GitHub App built with [Probot](https://github.com/probot/probot) that Generates a MD file with your DevOps Performance Metrics

## Steps
1. Ensure you have a Github Repo, where you have the ability to install applications
2. Make sure you have deployed the DevOpsMetrics Solution, as it is critical we can call the APIs in that project.
3. Checkin a .github/probotmetrics.yml, this file stores the locations of all the APIs that are in the DevOpsMetrics .netcore solution
4. Make sure you don't have a .env file in the solution if this is the first time you are running, probot will walk you through the app generation and create your .env file
5. npm install
6. npm start
7. Click on Register App
![ProbotSetup](https://github.com/samsmithnz/DevOpsMetrics/blob/master/ReadmeImages/ProbotSetup.png) 
8. Set the App Name
![ProbotSetup](https://github.com/samsmithnz/DevOpsMetrics/blob/master/ReadmeImages/ProbotAppSetup1.png) 
9. Select the repo you want to install
![ProbotSetup](https://github.com/samsmithnz/DevOpsMetrics/blob/master/ReadmeImages/ProbotAppSetup2.png) 
10. Ensure you have a .env
11. Restart the app

## License
Enjoy, but don't hold us accountable : )
© 2020 Chris Hanna <chris@chaxa.com>
