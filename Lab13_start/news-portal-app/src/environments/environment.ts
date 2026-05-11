// In Docker (production): nginx proxy serveste si /api/* si /, deci URL-uri relative.
// apiUrl gol -> ArticleService apeleaza '/api/articles' (relativ) -> nginx proxy_pass http://api:8080.
export const environment = {
  production: true,
  apiUrl: ''
};
