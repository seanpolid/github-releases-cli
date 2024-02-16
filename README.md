<h1>Overview</h1>
GitHub Releases CLI is a command line tool that focuses on automating the zipping and packaging of assets to be uploaded to the GitHub Releases endpoint. Follow the instructions below to get this integrated into your CI/CD pipeline.

<h1>Instructions</h1>
Regardless of the route you take to get this installed on your machine, you will need to set the following environmental variables for everything to work:
<ol>
  <li>GITHUB_TOKEN - A personal access token with the repo scope selected (this is the minimal scope required)</li>
  <li>REPO_OWNER - Name of the user who owns the repository</li>
</ol>

<h2>Docker</h2>
If you're comfortable with Docker, there is a dockerfile in this repo that you can work from. You can also pull the image directly:

docker pull seanpolidori0/github-releases-cli

<h2>Releases</h2>
-- To be added

<h2>Manual</h2>
If you decide to perform the installation manually, you need to complete the following:

-- To be added
