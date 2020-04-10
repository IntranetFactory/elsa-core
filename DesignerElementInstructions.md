Instructions on how to use designer-element component:

1) go to element directory via this command: cd "C:\Dev\elsa-core\designer-element"
2) run npm install
3) run npm build
4) run npm start

Every time the component is edited, run npm build and then npm start so that the /dist folder is copied to Elsa.Dashboard.Web wwwroot.


NOTE: designer-element folder can not be seen from Solution Explorer view so you have to switch the explorer to 'Folder View'.
      The reason for this is because Solution contains only solution items and we can only add 'Solution' types of folders into it.