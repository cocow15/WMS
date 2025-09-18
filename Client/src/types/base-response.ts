export type BaseResponse<T = unknown, P = unknown> = {
  code: number;
  success: boolean;
  data?: T | null;
  page?: P | null;
  errors?: string[] | null;
};
