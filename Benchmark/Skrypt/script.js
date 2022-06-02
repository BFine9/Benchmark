
const fs = require('fs');
var tsv = fs.readFileSync('worldcities.csv');

    function tsvJSON(tsv){        
    
        var lines=tsv.toString().split("\n");

        var result = [];
        var headers=lines[0].split(",");
        //var headers=lines[0].split("\t");
       
        for(var i=1;i<lines.length;i++){       
            var obj = {};
            var currentline=lines[i].split("\t");
            
        for(var j=0;j<headers.length;j++){
                obj[headers[j]] = currentline[j];
            }            
            console.debug(obj);
            result.push(obj);   
        };
    
    var path = 'json/cities.json';
    fs.writeFileSync(path,JSON.stringify(result));
    };
    tsvJSON(tsv);