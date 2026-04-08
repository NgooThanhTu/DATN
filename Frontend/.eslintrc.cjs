module.exports = {
  root: true,
  env: { browser: true, es2021: true },
  extends: ["plugin:vue/vue3-essential", "eslint:recommended"],
  parserOptions: { ecmaVersion: "latest", sourceType: "module" },
  rules: { "no-redeclare": "error" }
}
