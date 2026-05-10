export interface Article {
  id: number;
  title: string;
  content: string;
  publishedAt: string;       // ISO date string din JSON
  categoryId: number;
  categoryName: string;
  authorId: string;
  authorName: string;
  tags?: string[];           // optional - poate lipsi
}

export interface Category {
  id: number;
  name: string;
}

export interface CurrentUser {
  id: string;
  name: string;
  email: string;
  roles: string[];
}

export interface CreateArticleDto {
  title: string;
  content: string;
  categoryId: number;
}

export interface UpdateArticleDto {
  title: string;
  content: string;
  categoryId: number;
}
