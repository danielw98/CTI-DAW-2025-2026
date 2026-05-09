export interface Article {
  id: number;
  title: string;
  content: string;
  publishedAt: string;
  categoryId: number;
  categoryName: string;
  authorId: string;
  authorName: string;
  tags?: string[];
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
