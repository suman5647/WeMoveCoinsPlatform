var Service = require('node-windows').Service;
var svc = new Service({
  name:'BitGoJsTest',
  description: 'This is BitGo as a service',
  script: 'C:\\Apps\\Bitgo\\BitGoJS-masterTest\\bin\\bitgo-express'
});
svc.on('install',function(){
  svc.start();
});
svc.install();