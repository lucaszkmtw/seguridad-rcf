_VERSION = "";
_ROOT = document.getElementsByTagName("script")[0].getAttribute("src").substring(0, document.getElementsByTagName("script")[0].getAttribute("src").indexOf('/S'));
//document.getElementsByTagName("script")[0].getAttribute("src").substring(0, document.getElementsByTagName("script")[0].getAttribute("src").indexOf('/S'));

function loadDoc() {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function() {
        if (this.readyState == 4 && this.status == 200) {
            _VERSION = this.responseText;
        }
    };
    xhttp.open("GET", _ROOT+"/version.txt", true);
    xhttp.send();
}
loadDoc();
console.log(_VERSION);


var JSElement = document.createElement('script');
JSElement.src = _ROOT +"/Scripts/template/common.min.js";
//document.getElementsByTagName('body')[0].appendChild(JSElement);
document.getElementsByTagName('body')[0].insertBefore(JSElement, document.getElementsByTagName('body')[0].firstChild);

//JSElement = document.createElement('script');
//JSElement.src = _ROOT +"/Scripts/template/uikit_custom.min.js";
//document.getElementsByTagName('body')[0].appendChild(JSElement);

//JSElement = document.createElement('script');
//JSElement.src = _ROOT +"/Scripts/template/altair_admin_common.js" + _VERSION;
//document.getElementsByTagName('body')[0].appendChild(JSElement);


//JSElement = document.createElement('script');
//JSElement.src = _ROOT +"/Scripts/datatablesALTAIR/plugins/datatables.min.js" + _VERSION;
//document.getElementsByTagName('body')[0].appendChild(JSElement);

//JSElement = document.createElement('script');
//JSElement.src = _ROOT + "/Scripts/datatablesALTAIR/plugins/dataTables.uikit.min.js" + _VERSION;
//document.getElementsByTagName('body')[0].appendChild(JSElement);


//JSElement = document.createElement('script');
//JSElement.src = _ROOT + "/Scripts/datatablesALTAIR/plugins/plugins_datatables.min.js" + _VERSION;
//document.getElementsByTagName('body')[0].appendChild(JSElement);

//JSElement = document.createElement('script');
//JSElement.src = _ROOT + "/Scripts/datatablesALTAIR/plugins/date-de.js" + _VERSION;
//document.getElementsByTagName('body')[0].appendChild(JSElement);

//JSElement = document.createElement('script');
//JSElement.src = _ROOT + "/Scripts/sweetalert2.min.js" + _VERSION;
//document.getElementsByTagName('body')[0].appendChild(JSElement);

//JSElement = document.createElement('script');
//JSElement.src = _ROOT + "/Scripts/template/layoutScripts.js" + _VERSION;
//document.getElementsByTagName('body')[0].appendChild(JSElement);


