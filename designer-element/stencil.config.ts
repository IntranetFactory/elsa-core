import { Config } from '@stencil/core';
const { sass } = require('@stencil/sass');

export const config: Config = {
  namespace: 'elsa-workflow-designer',
  outputTargets: [
    {
      type: 'dist',
      dir: '../src/dashboard/Elsa.Dashboard/wwwroot/assets/js/plugins/elsa-workflows/',
      esmLoaderPath: '../../../../../../../../designer-element/loader'
    },
    {
      type: 'docs-readme'
    },
    {
      type: 'www',
      serviceWorker: null // disable service workers
    }
  ],
  plugins: [
    sass()
  ]
};
