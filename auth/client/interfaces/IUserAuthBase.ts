/**
 * ユーザー認証情報の基底インターフェース
 */
interface IUserAuthBase {
  /**
   * ユーザーID
   */
  userId: string;

  /**
   * ユーザー名
   */
  userName: string;

  /**
   * GoogleユーザーID
   */
  googleUserId: string;
}