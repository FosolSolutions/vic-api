ASPNETCORE_ENVIRONMENT=VS
ASPNETCORE_URLS=https://*:10443;http://*:1080

ASPNETCORE_Kestrel__Certificates__Default__Path=../certs/aspnetcore.pfx
ASPNETCORE_Kestrel__Certificates__Default__Password=password

Synology__Username=api
Synology__Password=Gbd($`

ConnectionStrings__VicWeb=SERVER=localhost;PORT=3306;DATABASE=vic-web;
DB_USERID=root
DB_PASSWORD=TL24admin!

Authentication__Issuer=https://localhost:10443/
Authentication__Audience=https://localhost:10443/
Authentication__Salt=6945fc7b43744c8aac70f720d888ca81
Authentication__Secret=6945fc7b43744c8aac70f720d888ca81

Cors__WithOrigins=http://localhost:3000 https://localhost:3000

Mail__Host=smtp.ethereal.email
Mail__Port=587
Mail__Name=Jamey Pfeffer
Mail__Username=jamey80@ethereal.email
Mail__Password=RVGuggfkSe9ekHzVg1
Mail__FromEmail=contact@victoriabiblestudy.com
Mail__ContactEmail=jeremymfoster@hotmail.com