<script setup>

</script>
<template>
    <div class="container">
        <img src="../assets/access-denied.png" alt="">
        <h4>Không thể truy cập vào trang !</h4>
    </div>
</template>
<style scoped>
.container {
    width: 100%;
    height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
}
.container img {
    display: block;
    width: 100%;
}

.container h4 {
    margin-top: 24px;
    font-weight: 600;
    font-size: 20px;
}
</style>