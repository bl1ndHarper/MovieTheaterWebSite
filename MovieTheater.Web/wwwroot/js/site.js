function animateBubbles() {
  const scrollY = window.scrollY || 0;
  const time = performance.now() / 1000;

  document.querySelectorAll('.bubble').forEach((bubble, i) => {
    const depth = parseFloat(bubble.dataset.depth || 0.2);
    const float = Math.sin(time + i) * 12;

    const offset = -scrollY * depth;
    bubble.style.transform = `translateY(${offset + float}px)`;
  });

  requestAnimationFrame(animateBubbles);
}
animateBubbles();