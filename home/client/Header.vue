<template>
  <nav class="navbar">
    <!-- 左側のメニュー -->
    <div class="menu-left">
      <b-button
        v-for="(item, index) in leftMenuItems"
        :key="index"
        type="is-primary"
        size="is-medium"
        tag="a"
        :href="item.link"
      >
        {{ item.label }}
      </b-button>
    </div>

    <!-- 右側のメニュー -->
    <div class="menu-right">
      <template v-if="UserName">
        Welcome, {{ UserName }}!!!
      </template>
      <template v-else>
        <b-button
          v-for="(item, index) in RightMenuItems"
          :key="index"
          type="is-light"
          size="is-medium"
          tag="a"
          :href="item.link"
        >
          {{ item.label }}
        </b-button>
      </template>
    </div>
  </nav>
</template>

<script lang="ts">
import AuthUtil from "@auth/utils/AuthUtil";
import { Component, Vue, toNative } from "vue-facing-decorator";

interface MenuItem {
  label: string;
  link: string;
}

@Component
class Header extends Vue {
  /**
   * 左側のメニューボタン
   */
  public leftMenuItems: MenuItem[] = [
    { label: "Home", link: "/" },
    { label: "Privacy", link: "/Home/Privacy" },
  ];

  /**
   * ユーザー情報
   */
  private user: IUserAuthBase | null = null;

  /**
   * 右側のメニューボタン
   */
  public get RightMenuItems(): MenuItem[] {
    if (this.user === null) {
      return [
        { label: "Login", link: "/Account/Login" },
        { label: "Register", link: "/Account/Register" },
      ];
    } else if (this.user.userName === '') {
      return [
        { label: "Register", link: "/Account/Register" },
      ];
    } else {
      return [];
    }
  }

  /**
   * ユーザー名
   */
  public get UserName(): string {
    return this.user?.userName ?? "";
  }

  /**
   * コンポーネントが作成されたときに呼び出されるライフサイクルフック
   */
  public async created() {
    this.user = await AuthUtil.GetUser<IUserAuthBase>();
  }
}

export default toNative(Header)
</script>

<style scoped>
.navbar {
  display: flex; /* フレックスボックスで全体を整列 */
  justify-content: space-between; /* 左と右にボタンを分ける */
  align-items: center; /* 縦方向で中央揃え */
  padding: 10px 20px; /* ナビゲーションバーの余白を追加 */
  background-color: #ffffff; /* 背景色を白に設定 */
  border-bottom: 1px solid #ddd; /* 下に区切り線 */
}

.menu-left,
.menu-right {
  display: flex; /* フレックスボックスでボタンを並べる */
  gap: 10px; /* ボタン同士の間隔を10pxに設定 */
}

.menu-left {
  justify-content: flex-start; /* 左側ボタンを左に寄せる */
}

.menu-right {
  justify-content: flex-end; /* 右側ボタンを右に寄せる */
}
</style>
