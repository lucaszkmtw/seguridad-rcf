/*
Char  |  Number	 |   Description
-------------------------------
NUL	    00	    null character
SOH	    01	    start of header
STX	    02	    start of text
ETX	    03	    end of text
EOT	    04	    end of transmission
ENQ	    05	    enquiry
ACK	    06	    acknowledge
BEL	    07	    bell (ring)
BS	    08	    backspace
HT	    09	    horizontal tab
LF	    10	    line feed
VT	    11	    vertical tab
FF	    12	    form feed
CR	    13	    carriage return
SO	    14	    shift out
SI	    15	    shift in
DLE	    16	    data link escape
DC1	    17	    device control 1
DC2	    18	    device control 2
DC3	    19	    device control 3
DC4	    20	    device control 4
NAK	    21	    negative acknowledge
SYN	    22	    synchronize
ETB	    23	    end transmission block
CAN	    24	    cancel
EM	    25	    end of medium
SUB	    26	    substitute
ESC	    27	    escape
FS	    28	    file separator
GS	    29	    group separator
RS	    30	    record separator
US	    31	    unit separator
 	    32	    space
!	    33	    exclamation mark
"	    34	    quotation mark
#	    35	    number sign
$	    36	    dollar sign
%	    37	    percent sign
&	    38	    ampersand
'	    39	    apostrophe
(	    40	    left parenthesis
)	    41	    right parenthesis
*	    42	    asterisk
+	    43	    plus sign
,	    44	    comma
-	    45	    hyphen
.	    46	    period
/	    47	    slash
 */
const charBackspace = 8;
const charNul = 0;
const charSupr = 13;
const charComma = 44;
const charPoint = 46;

_PREV_COMMA_DIGIT = 0;
_SPLIT_STRING = "";
function keypressImporte(event) {    
    var input = $(this);
    var oldVal = input.val();
    
    var regex = new RegExp("^\\d*((\\.|\\,)\\d{0,2})?$", 'g');

    setTimeout(function () {
        var newVal = input.val();
        if (!regex.test(newVal)) {
            input.val(oldVal);
        }
    }, 0);
}

function verifyAfterPoint(elemtn) {
    let aux = $(elemtn).val().split(_SPLIT_STRING);
    if (aux[1] != undefined && aux[1].length >= 2 && _PREV_COMMA_DIGIT != 2) {
        event.preventDefault();
        _PREV_COMMA_DIGIT = (!isNullOrEmpty(aux[1])) ? aux[1].length : 0;
    }
}