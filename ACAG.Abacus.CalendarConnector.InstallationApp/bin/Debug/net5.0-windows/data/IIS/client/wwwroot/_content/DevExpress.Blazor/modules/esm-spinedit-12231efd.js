import"./esm-chunk-d81494b9.js";import{ensureElement as e,getParentByClassName as t}from"./esm-dom-utils-c40cd314.js";import{a as o,d as n}from"./esm-chunk-9c16a801.js";import{d as s,r as a}from"./esm-chunk-f3c18c5b.js";import{K as r}from"./esm-chunk-808bf349.js";function i(e,t){e&&(e.dataset.previousValue=t)}function d(i,d,c){i=e(i);const u=t(i,"dxbs-spin-edit");if(!i)return;s(u);const l=(f=d.decimalSeparator,p=d.needExponentialView,m=i,function(e){let t=m.dataset.selectionStartBeforePaste?m.value.trim():m.value,o=/^-?(\d)*$/;f&&(t=t.replace(/[.|,]/g,f),o=p?/^-?(\d+|[,.]\d+|\d+[,.]\d+|\d+[,.]|[,.])?([eE]?[+-]?(\d)*)?$/:/^-?(\d+|[,.]\d+|\d+[,.]\d+|\d+[,.]|[,.])?$/);let n=m.selectionStart,s=m.selectionEnd;o.test(t)?(t!==m.value&&(m.value=t),m.dataset.previousValue=t):(m.dataset.selectionStartBeforePaste?(n=m.dataset.selectionStartBeforePaste,s=m.dataset.selectionEndBeforePaste):(n--,s+=m.dataset.previousValue.length-t.length),m.value=m.dataset.previousValue),m.dataset.selectionStartBeforePaste&&(m.removeAttribute("data-selection-start-before-paste"),m.removeAttribute("data-selection-end-before-paste")),m.selectionStart=n,m.selectionEnd=s});var f,p,m;const v=function(e){return function(t){e.dataset.selectionStartBeforePaste=e.selectionStart,e.dataset.selectionEndBeforePaste=e.selectionEnd}}(i),k=function(e){if(function(e){return e.keyCode===r.Up||e.keyCode===r.Down}(e)){if(e.stopPropagation(),e.preventDefault(),e.repeat)return;c.invokeMethodAsync("OnKeyDown",e.keyCode===r.Up,i.value)}};return o(i,"input",l),o(i,"paste",v),o(i,"keydown",k),a(u,(function(){n(i,"input",l),n(i,"keydown",k),n(i,"paste",v)})),Promise.resolve("ok")}function c(e,t){let o=document.activeElement;for(;null!==o&&o!==e;)o=o.parentElement;null===o&&t.invokeMethodAsync("FocusLost")}function u(t){if(t=e(t))return s(t),Promise.resolve("ok")}const l={init:d,dispose:u,setPreviousValue:i,checkFocusedState:c};export default l;export{c as checkFocusedState,u as dispose,d as init,i as setPreviousValue};