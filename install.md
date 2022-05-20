az ad sp create-for-rbac --name "Dora" --role contributor \
                            --scopes /subscriptions/625b66d7-5b11-40fb-99ab-ba303c13ea88/resourceGroups/DoraMetrics-wes-dev-rg \
                            --sdk-auth