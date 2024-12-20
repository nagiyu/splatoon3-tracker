import { defineFlatConfig } from 'eslint-define-config';
import vue from 'eslint-plugin-vue';
import typescriptParser from '@typescript-eslint/parser';
import typescriptPlugin from '@typescript-eslint/eslint-plugin';
import vueParser from 'vue-eslint-parser'; // Vueパーサーを明示的にインポート

export default defineFlatConfig([
  {
    // グローバル設定
    languageOptions: {
      parser: vueParser, // パーサーはインポートしたオブジェクトを渡す
      parserOptions: {
        parser: typescriptParser, // TypeScriptパーサーを追加で指定
        ecmaVersion: 2021,
        sourceType: 'module',
      },
      globals: {
        window: true,
        document: true,
        process: true,
        console: true,
      },
    },
    plugins: {
      vue,
      '@typescript-eslint': typescriptPlugin,
    },
    rules: {
      'no-unused-vars': 'warn',
      'no-console': 'off',
    },
  },
  {
    // Vueファイル用設定
    files: ['**/*.vue'],
    rules: {
      ...vue.configs['vue3-recommended'].rules,
      'vue/html-indent': ['error', 2],
    },
  },
  {
    // TypeScriptファイル用設定
    files: ['**/*.ts'],
    rules: {
      '@typescript-eslint/no-unused-vars': ['warn'],
    },
  },
]);
