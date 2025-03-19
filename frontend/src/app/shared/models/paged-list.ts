export class PagedList<T> {
    items: T[] = [];
    pageNumber = 1;
    pageSize = 10;
    totalCount = 0;
    totalPages = 0;
}