module.exports = async (context, {
    owner, repo, title, body, labels,
  }) => {
    // method is used to create issues
    context.log.debug('In postCreateIssues.js...');
    return context.github.issues.create({
      owner,
      repo,
      title,
      body,
      labels,
    });
  };
  