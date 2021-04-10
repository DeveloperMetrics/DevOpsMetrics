﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

//[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "lower case properties are returned from API's, ignoring this rule for the entire Models project", Scope = "namespaceanddescendants", Target = "DevOpsMetrics.Core.Models")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "This is a 'silly' rule. It doesn't make sense.", Scope = "member", Target = "~M:DevOpsMetrics.Core.DataAccess.APIAccess.AzureDevOpsAPIAccess.GetAzureDevOpsMessage(System.String,System.String)~System.Threading.Tasks.Task{System.String}")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "This is a 'silly' rule. It doesn't make sense.", Scope = "member", Target = "~M:DevOpsMetrics.Core.DataAccess.APIAccess.GitHubAPIAccess.GetGitHubMessage(System.String,System.String,System.String)~System.Threading.Tasks.Task{System.String}")]
//[assembly: SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "This is a 'silly' rule. The switch is clearer.", Scope = "member", Target = "~M:DevOpsMetrics.Core.Models.Common.Badges.BadgeColor(System.String)~System.String")]
[assembly: SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "This is a 'silly' rule. The switch is clearer.", Scope = "member", Target = "~M:DevOpsMetrics.Core.Models.Common.Badges.BadgeURL(System.String,System.String)~System.String")]
