export interface Article {
  id: number;
  title: string;
  content: string;
  publishedAt: string;
  categoryId: number;
  categoryName: string;
  authorId: string | null;
  authorName: string;
  imagePath?: string | null;
  tags: Tag[];
}

export interface Category {
  id: number;
  name: string;
}

export interface Tag {
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
  tagIds?: number[];
}

export interface UpdateArticleDto {
  title: string;
  content: string;
  categoryId: number;
  tagIds?: number[];
}
