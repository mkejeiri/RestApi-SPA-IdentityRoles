// pagination header that we get from the request
export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}
// what we will get back from the request
export class PaginationResult<T> {
    result: T;
    pagination: Pagination;
}
