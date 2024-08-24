docker exec contosoOpenLdap ldapmodify `
-a -x -h localhost -p 389 `
-D "cn=admin,dc=contoso,dc=com" `
-f /data/ldif/00-startup.ldif `
-w P@ss1W0Rd! `
-c

docker exec contosoOpenLdap ldapmodify `
-a -x -h localhost -p 389 `
-D "cn=admin,dc=contoso,dc=com" `
-f /data/ldif/01-output-groups.ldif `
-w P@ss1W0Rd! `
-c

docker exec contosoOpenLdap ldapmodify `
-a -x -h localhost -p 389 `
-D "cn=admin,dc=contoso,dc=com" `
-f /data/ldif/02-output-users.ldif `
-w P@ss1W0Rd! `
-c