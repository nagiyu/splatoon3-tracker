import axios, { AxiosResponse } from 'axios';

/**
 * Interface for the user authentication base.
 */
export default class AuthUtil {
  public static async GetUser<T extends IUserAuthBase>(): Promise<T | null> {
    var response = await axios.get<any, AxiosResponse<T, any>>('/api/account/user');

    if (response.status === 200) {
      return response.data;
    }

    return null;
  }
}