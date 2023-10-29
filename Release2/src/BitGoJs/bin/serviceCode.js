var Service = require('node-windows').Service;
var svc = new Service({
  name:'BitGoJs',
  description: 'This is BitGo as a service',
  script: 'C:\\Apps\\Bitgo\\BitGoJS-master\\bin\\bitgo-express'
});
svc.on('install',function(){
  svc.start();
});
svc.install();